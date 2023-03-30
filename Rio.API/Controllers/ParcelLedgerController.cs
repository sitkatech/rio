using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Models;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ParcelLedgerController : SitkaController<ParcelLedgerController>
    {
        private readonly bool _includeWaterSupply;

        public ParcelLedgerController(RioDbContext dbContext, ILogger<ParcelLedgerController> logger,
            KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger,
            keystoneService, rioConfiguration)
        {
            _includeWaterSupply = _rioConfiguration.IncludeWaterSupply;
        }

        [HttpGet("parcel-ledgers/transaction-history")]
        [ParcelViewFeature]
        public ActionResult<List<TransactionHistoryDto>> ListTransactionHistory()
        {
            var transactionHistoryDtos = ParcelLedgers.ListTransactionHistoryAsDto(_dbContext);

            return Ok(transactionHistoryDtos);
        }

        [HttpPost("parcel-ledgers")]
        [ParcelManageFeature]
        public IActionResult New([FromBody] ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            if (!_includeWaterSupply && parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.Supply)
            {
                return Forbid();
            }

            var parcelNumber = parcelLedgerCreateDto.ParcelNumbers?.Single();
            if (string.IsNullOrWhiteSpace(parcelNumber))
            {
                ModelState.AddModelError("ParcelNumber", "Please enter a valid Parcel APN.");
            }
            if (parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.Supply && parcelLedgerCreateDto.WaterTypeID == null)
            {
                ModelState.AddModelError("SupplyType", "The Supply Type field is required for transactions adjusting water supply.");
            }
            ValidateEffectiveDate(parcelLedgerCreateDto.EffectiveDate);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parcelDto = Parcel.GetByParcelNumberAsDto(_dbContext, parcelNumber);
            if (parcelDto == null)
            {
                ModelState.AddModelError("ParcelNumber", $"{parcelNumber} is not a valid Parcel APN.");
            }
            if (parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.Usage)
            {
                // flip TransactionAmount sign for usage adjustment; usage is negative in the ledger, but a user-inputted positive value should increase usage sum (and vice versa)
                parcelLedgerCreateDto.TransactionAmount *= -1;
                ValidateUsageAmount(parcelLedgerCreateDto, parcelDto.ParcelID);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            ParcelLedgers.CreateNew(_dbContext, parcelDto, parcelLedgerCreateDto, userDto.UserID);
            
            return Ok();
        }

        [HttpPost("parcel-ledgers/bulk")]
        [ParcelManageFeature]
        public IActionResult BulkNew([FromBody] ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            if (!_includeWaterSupply)
            {
                return Forbid();
            }

            if (parcelLedgerCreateDto.WaterTypeID == null)
            {
                ModelState.AddModelError("SupplyType", "The Supply Type field is required.");
                return BadRequest(ModelState);
            }
            ValidateEffectiveDate(parcelLedgerCreateDto.EffectiveDate);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var postingCount = ParcelLedgers.BulkCreateNew(_dbContext, parcelLedgerCreateDto, userDto.UserID);
            return Ok(postingCount);
        }

        [HttpPost("/parcel-ledgers/csv")]
        [RequestSizeLimit(524288000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        public async Task<IActionResult> NewCSVUpload([FromForm] ParcelLedgerCsvUpsertDto parcelLedgerCsvUpsertDto)
        {
            if (!_includeWaterSupply)
            {
                return Forbid();
            }

            var fileResource = await HttpUtilities.MakeFileResourceFromIFormFile(parcelLedgerCsvUpsertDto.UploadedFile, _dbContext, HttpContext);
            var waterTypeDisplayName =
                _dbContext.WaterTypes.Single(x => x.WaterTypeID == parcelLedgerCsvUpsertDto.WaterTypeID).WaterTypeName;

            ValidateEffectiveDate(parcelLedgerCsvUpsertDto.EffectiveDate);
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
            var effectiveDate = DateTime.Parse(parcelLedgerCsvUpsertDto.EffectiveDate);
            var postingCount = ParcelLedgers.CreateNewFromCSV(_dbContext, records, fileResource.OriginalBaseFilename, effectiveDate, parcelLedgerCsvUpsertDto.WaterTypeID.Value, userDto.UserID);
            return Ok(postingCount);
        }

        private bool ParseCSVUpload(FileResource fileResource, string waterTypeDisplayName, out List<ParcelLedgerCreateCSV> records)
        {
            try
            {
                using var memoryStream = new MemoryStream(fileResource.FileResourceData);
                using var reader = new StreamReader(memoryStream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                csvReader.Context.RegisterClassMap(new ParcelLedgerCreateCSVMap(_dbContext, waterTypeDisplayName));
                csvReader.Read();
                csvReader.ReadHeader();
                var headerNamesDuplicated =
                    csvReader.HeaderRecord.Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).Where(x => x.Count() > 1).ToList();
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

            // no duplicate APNs
            var duplicateAPNs = records.GroupBy(x => x.APN).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();

            if (duplicateAPNs.Any())
            {
                var singularOrPluralAPN = (duplicateAPNs.Count > 1 ? "these APNs" : "this APN");
                ModelState.AddModelError("UploadedFile", 
                    $"The uploaded file contains multiples rows with {singularOrPluralAPN}: {string.Join(", ", duplicateAPNs)}");
                return false;
            }

            // all existing APNs
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

        private void ValidateEffectiveDate(string effectiveDate)
        {
            var effectiveDateAsDateTime = DateTime.Parse(effectiveDate);

            var earliestWaterYear = WaterYear.List(_dbContext).OrderBy(x => x.Year).First();
            if (effectiveDateAsDateTime.Year < earliestWaterYear.Year)
            {
                ModelState.AddModelError("EffectiveDate", $"Transactions for dates before 1/1/{earliestWaterYear.Year} are not allowed");
            }

            var currentDate = DateTime.UtcNow;
            if (DateTime.Compare(effectiveDateAsDateTime, currentDate) > 0)
            {
                ModelState.AddModelError("EffectiveDate", "Transactions for future dates are not allowed.");
            }
        }

        private void ValidateUsageAmount(ParcelLedgerCreateDto parcelLedgerCreateDto, int parcelID)
        {
            var effectiveDate = DateTime.Parse(parcelLedgerCreateDto.EffectiveDate);
            if (parcelLedgerCreateDto.TransactionAmount > 0)
            {
                var monthlyUsageSum = ParcelLedgers.GetUsageSumForMonthAndParcelID(_dbContext, effectiveDate.Year, effectiveDate.Month, parcelID);
                if (parcelLedgerCreateDto.TransactionAmount + monthlyUsageSum > 0)
                {
                    ModelState.AddModelError("TransactionAmount", 
                        $"Parcel usage for {effectiveDate.Month}/{effectiveDate.Year} is currently {Math.Round(monthlyUsageSum, 2)}. Usage correction quantity cannot exceed total usage for month.");
                }
            }
        }
    }
}