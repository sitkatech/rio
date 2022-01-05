using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using ParcelLedgerCreateCSVUploadDto = Rio.API.Models.ParcelLedgerCreateCSVUploadDto;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ParcelLedgerController : SitkaController<ParcelLedgerController>
    {
        public ParcelLedgerController(RioDbContext dbContext, ILogger<ParcelLedgerController> logger,
            KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger,
            keystoneService, rioConfiguration)
        {
        }

        [HttpPost("parcel-ledgers/new")]
        [ParcelManageFeature]
        public IActionResult New([FromBody] ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            var parcelDto = Parcel.GetByParcelNumberAsDto(_dbContext, parcelLedgerCreateDto.ParcelNumbers[0]); 
            if (parcelDto == null)
            {
                ModelState.AddModelError("ParcelNumber", $"{parcelLedgerCreateDto.ParcelNumbers[0]} is not a valid Parcel APN.");
                return BadRequest(ModelState);
            }

            ValidateEffectiveDate(parcelLedgerCreateDto.EffectiveDate);
            if (parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.Usage)
            {
                // flip TransactionAmount sign for usage adjustment; usage is negative in the ledgera user-inputted positive value should increase usage sum (and vice versa)
                parcelLedgerCreateDto.TransactionAmount *= -1;
                ValidateUsageAmount(parcelLedgerCreateDto);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            ParcelLedgers.CreateNew(_dbContext, parcelDto, parcelLedgerCreateDto, userDto.UserID);
            return Ok();
        }

        [HttpPost("parcel-ledgers/bulk-new")]
        [ParcelManageFeature]
        public IActionResult BulkNew([FromBody] ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            ValidateEffectiveDate(parcelLedgerCreateDto.EffectiveDate);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var postingCount = ParcelLedgers.BulkCreateNew(_dbContext, parcelLedgerCreateDto, userDto.UserID);
            return Ok(postingCount);
        }

        [HttpPost("/parcel-ledgers/new-csv-upload")]
        [RequestSizeLimit(524288000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        public async Task<IActionResult> NewCSVUpload([FromForm] ParcelLedgerCreateCSVUploadDto parcelLedgerCreateCSVUploadDto)
        {
            var fileResource = await HttpUtilities.MakeFileResourceFromIFormFile(parcelLedgerCreateCSVUploadDto.UploadedFile, _dbContext, HttpContext);
            var waterTypeDisplayName =
                _dbContext.WaterTypes.Single(x => x.WaterTypeID == parcelLedgerCreateCSVUploadDto.WaterTypeID).WaterTypeName;

            ValidateEffectiveDate(parcelLedgerCreateCSVUploadDto.EffectiveDate);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!ParseCSVUpload(fileResource, waterTypeDisplayName, out var records))
            {
                return BadRequest(ModelState);
            }

            if (!ValidateCSVUpload(records, waterTypeDisplayName))
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var postingCount = ParcelLedgers.CreateNewFromCSV(_dbContext, records, fileResource.OriginalBaseFilename, parcelLedgerCreateCSVUploadDto.EffectiveDate, parcelLedgerCreateCSVUploadDto.WaterTypeID, userDto.UserID);
            return Ok(postingCount);
        }

        private bool ParseCSVUpload(FileResource fileResource, string waterTypeDisplayName, out List<ParcelLedgerCreateCSV> records)
        {
            try
            {
                using var memoryStream = new MemoryStream(fileResource.FileResourceData);
                using var reader = new StreamReader(memoryStream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                csvReader.Configuration.RegisterClassMap(new ParcelLedgerCreateCSVMap(_dbContext, waterTypeDisplayName));
                csvReader.Read();
                csvReader.ReadHeader();
                var headerNamesDuplicated =
                    csvReader.Context.HeaderRecord.Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).Where(x => x.Count() > 1).ToList();
                if (headerNamesDuplicated.Any())
                {
                    var singularOrPluralName = (headerNamesDuplicated.Count > 1 ? "names appear" : "name appears");
                    ModelState.AddModelError("UploadedFile",
                        $"The following header {singularOrPluralName} more than once: {string.Join(", ", headerNamesDuplicated.OrderBy(x => x.Key).Select(x => x.Key))}");
                    records = null;
                    return false;
                }

                records = csvReader.GetRecords<ParcelLedgerCreateCSV>().ToList();
            }
            catch (HeaderValidationException e)
            {
                var headerMessage = e.Message.Split('.')[0];
                ModelState.AddModelError("UploadedFile",
                    $"{headerMessage}. Please check that the column name is not missing or misspelled.");
                records = null;
                return false;
            }
            catch (CsvHelper.MissingFieldException e)
            {
                var headerMessage = e.Message.Split('.')[0];
                ModelState.AddModelError("UploadedFile",
                    $"{headerMessage}. Please check that the column name is not missing or misspelled.");
                records = null;
                return false;
            }
            catch
            {
                ModelState.AddModelError("UploadedFile",
                    "There was an error parsing the CSV. Please ensure the file is formatted correctly.");
                records = null;
                return false;
            }

            return true;
        }

        private bool ValidateCSVUpload(List<ParcelLedgerCreateCSV> records, string waterTypeDisplayName)
        {
            // no null APNs
            var nullAPNsCount = records.Count(x => x.APN == "");
            if (nullAPNsCount > 0)
            {
                var singularOrPluralRow = (nullAPNsCount > 1 ? "rows" : "row");
                ModelState.AddModelError("UploadedFile",
                    $"The uploaded file contains {nullAPNsCount} {singularOrPluralRow} specifying a value with no corresponding APN.");
                return false;
            }

            // no null quantities
            var nullQuantities = records.Where(x => x.Quantity == null).ToList();
            if (nullQuantities.Any())
            {
                var singularOrPluralAPN = (nullQuantities.Count > 1 ? "APNs" : "APN");
                ModelState.AddModelError("UploadedFile",
                    $"The following {singularOrPluralAPN} had no {waterTypeDisplayName} Quantity entered: {string.Join(", ", nullQuantities.Select(x => x.APN))}");
                return false;
            }

            // no duplicate APNs permitted
            var duplicateAPNs = records.GroupBy(x => x.APN).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();

            if (duplicateAPNs.Any())
            {
                var singularOrPluralAPN = (duplicateAPNs.Count > 1 ? "these APNs" : "this APN");
                ModelState.AddModelError("UploadedFile", 
                    $"The uploaded file contains multiples rows with {singularOrPluralAPN}: {string.Join(", ", duplicateAPNs)}");
                return false;
            }

            // all parcel numbers must match an existing Parcel record
            var allParcelNumbers = _dbContext.Parcels.Select(y => y.ParcelNumber);
            var unmatchedRecords = records.Where(x => !allParcelNumbers.Contains(x.APN)).ToList();

            if (unmatchedRecords.Any())
            {
                var singularOrPluralAPN = (unmatchedRecords.Count > 1 ? "these APNs which do" : "this APN which does");
                ModelState.AddModelError("UploadedFile", 
                    $"The uploaded file contains {singularOrPluralAPN} not match any record in the system: {string.Join(", ", unmatchedRecords.Select(x => x.APN))}");
                return false;
            }

            return true;
        }

        private void ValidateEffectiveDate(DateTime effectiveDate)
        {
            var earliestWaterYear = WaterYear.List(_dbContext).OrderBy(x => x.Year).First();
            if (effectiveDate.Year < earliestWaterYear.Year)
            {
                ModelState.AddModelError("EffectiveDate", 
                    $"Transactions for dates before 1/1/{earliestWaterYear.Year} are not allowed");
            }

            var currentDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            if (DateTime.Compare(effectiveDate, currentDate) > 0)
            {
                ModelState.AddModelError("EffectiveDate", "Transactions for future dates are not allowed.");
            }
        }

        private void ValidateUsageAmount(ParcelLedgerCreateDto parcelLedgerCreateDto)
        { 
            if (parcelLedgerCreateDto.TransactionAmount > 0)
            {
                var monthlyUsageSum = ParcelLedgers.GetUsageSumForMonthAndParcelID(_dbContext, parcelLedgerCreateDto.EffectiveDate.Year, parcelLedgerCreateDto.EffectiveDate.Month, parcelLedgerCreateDto.ParcelNumbers[0][0]);
                if (parcelLedgerCreateDto.TransactionAmount + monthlyUsageSum > 0)
                {
                    ModelState.AddModelError("TransactionAmount", 
                        $"Parcel usage for {parcelLedgerCreateDto.EffectiveDate.Month}/{parcelLedgerCreateDto.EffectiveDate.Year} is currently {Math.Round(monthlyUsageSum, 2)}. Usage correction quantity cannot exceed total usage for month.");
                }
            }
        }
    }
}