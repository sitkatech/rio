using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.EFModels.Entities;
using Rio.API.Models;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;


namespace Rio.API.Controllers;

[ApiController]
public class ParcelUsageController : SitkaController<ParcelUsageController>
{
    public ParcelUsageController(RioDbContext dbContext, ILogger<ParcelUsageController> logger, KeystoneService keystoneService, 
        IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration) { }

    [HttpGet("parcel-usages")]
    [ParcelManageFeature]
    public ActionResult<ParcelUsageStagingPreviewDto> GetStagedParcelUsagesForCurrentUser()
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;

        var stagedParcelUsages = ParcelUsages.GetByUserID(_dbContext, userID).ToList();
        var parcelUsageStagingSimpleDtos = stagedParcelUsages
            .Select(x => x.AsSimpleDto()).ToList();

        var stagedParcelUsagesParcelIDs = stagedParcelUsages.Select(x => x.ParcelID);
        var existingParcels = _dbContext.Parcels.AsNoTracking();

        var parcelNumbersWithoutStagedUsages = existingParcels
            .Where(x => !stagedParcelUsagesParcelIDs.Contains(x.ParcelID))
            .Select(x => x.ParcelNumber).ToList();

        var parcelUsageStagingPreviewDto =
            new ParcelUsageStagingPreviewDto(parcelUsageStagingSimpleDtos, parcelNumbersWithoutStagedUsages);

        return Ok(parcelUsageStagingPreviewDto);
    }

    [HttpPost("parcel-usages")]
    [ParcelManageFeature]
    public ActionResult<int> PublishStagedParcelUsagesForCurrentUser()
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        var transactionCount = ParcelUsages.PublishStagingToParcelLedgerByUserID(_dbContext, userID);

        return Ok(transactionCount);
    }

    [HttpDelete("parcel-usages")]
    [ParcelManageFeature]
    public ActionResult DeleteStagedParcelUsagesForCurrentUser()
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        ParcelUsages.DeleteFromStagingByUserID(_dbContext, userID);

        return Ok();
    }

    [HttpPost("parcel-usages/csv/headers")]
    [ParcelManageFeature]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    public async Task<IActionResult> ListCSVHeaders([FromForm] CsvUpsertDto csvUpsertDto, [FromRoute] int geographyID)
    {
        var extension = Path.GetExtension(csvUpsertDto.UploadedFile.FileName);
        if (extension != ".csv")
        {
            ModelState.AddModelError("FileResource", $"{extension[1..].ToUpper()} is not an accepted file extension");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var fileData = await HttpUtilities.GetIFormFileData(csvUpsertDto.UploadedFile);

        if (!ListHeadersFromCsvUpload(fileData, out var headerNames))
        {
            return BadRequest(ModelState);
        }

        return Ok(headerNames);
    }

    [HttpPost("parcel-usages/csv")]
    [ParcelManageFeature]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    public async Task<ActionResult<List<string>>> NewCSVUpload([FromForm] ParcelUsageCsvUpsertDto parcelUsageCsvUpsertDto, [FromRoute] int geographyID)
    {
        var extension = Path.GetExtension(parcelUsageCsvUpsertDto.UploadedFile.FileName);
        if (extension != ".csv")
        {
            ModelState.AddModelError("FileResource", $"{extension[1..].ToUpper()} is not an accepted file extension");
            return BadRequest(ModelState);
        }
        if (parcelUsageCsvUpsertDto.apnColumnName == parcelUsageCsvUpsertDto.quantityColumnName)
        {
            ModelState.AddModelError("Value Column", "The selected Value column cannot match the selected APN column. Two distinct header names are required.");
            return BadRequest(ModelState);
        }

        var fileData = await HttpUtilities.GetIFormFileData(parcelUsageCsvUpsertDto.UploadedFile);

        if (!ParseCsvUpload(fileData, parcelUsageCsvUpsertDto.apnColumnName, parcelUsageCsvUpsertDto.quantityColumnName, out var records)
            || !ValidateCsvUploadData(records, geographyID))
        {
            return BadRequest(ModelState);
        }

        var effectiveDate = DateTime.Parse(parcelUsageCsvUpsertDto.EffectiveDate).AddHours(8);
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;

        var unmatchedParcelNumbers = ParcelUsages.CreateStagingRecordsFromCsv(_dbContext, records, effectiveDate, parcelUsageCsvUpsertDto.UploadedFile.FileName, userID);

        return Ok(unmatchedParcelNumbers);
    }

    private bool ListHeadersFromCsvUpload(byte[] fileData, out List<string> headerNames)
    {
        try
        {
            using var memoryStream = new MemoryStream(fileData);
            using var reader = new StreamReader(memoryStream);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            csvReader.Context.RegisterClassMap(new ParcelTransactionCSVMap("APN", "Usage"));
            csvReader.Read();
            csvReader.ReadHeader();

            headerNames = null;

            if (csvReader.HeaderRecord.Count(x => !string.IsNullOrWhiteSpace(x)) == 1)
            {
                ModelState.AddModelError("UploadedFile", "The uploaded CSV only contains one header name. At least two named columns are required.");
                return false;
            }

            var headerNamesDuplicated = csvReader.HeaderRecord.Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).Where(x => x.Count() > 1).ToList();
            if (headerNamesDuplicated.Any())
            {
                ModelState.AddModelError("UploadedFile",
                    $"The following header {(headerNamesDuplicated.Count > 1 ? "names appear" : "name appears")} more than once: {string.Join(", ", headerNamesDuplicated.OrderBy(x => x.Key).Select(x => x.Key))}");
                return false;
            }

            headerNames = csvReader.HeaderRecord.ToList();
            return true;
        }
        catch
        {
            ModelState.AddModelError("UploadedFile",
                "There was an error parsing the CSV. Please ensure the file is formatted correctly.");
            headerNames = null;
            return false;
        }
    }

    private bool ParseCsvUpload(byte[] fileData, string apnColumnName, string quantityColumnName, out List<ParcelTransactionCSV> records)
    {
        try
        {
            using var memoryStream = new MemoryStream(fileData);
            using var reader = new StreamReader(memoryStream);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            csvReader.Context.RegisterClassMap(new ParcelTransactionCSVMap(apnColumnName, quantityColumnName));
            records = csvReader.GetRecords<ParcelTransactionCSV>().ToList();
            return true;
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
    }

    private bool ValidateCsvUploadData(List<ParcelTransactionCSV> records, out int nullAPNsCount)
    {
        var isValid = true;

        // count null APNs
         nullAPNsCount = records.Count(x => x.APN == "");
        

        // no null quantities
        var nullQuantities = records.Where(x => x.Quantity == null).ToList();
        if (nullQuantities.Any())
        {
            ModelState.AddModelError("UploadedFile",
                $"The following {(nullQuantities.Count > 1 ? "APNs" : "APN")} had no usage quantity entered: {string.Join(", ", nullQuantities.Select(x => x.APN))}");
            isValid = false;
        }

        return isValid;
    }
}