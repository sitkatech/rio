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
using Rio.API.Controllers;
using Rio.API.Models;
using Rio.API.Services;
using Rio.EFModels.Entities;


namespace Rio.API.Controllers;

[ApiController]
public class ParcelUsageController : SitkaController<ParcelUsageController>
{
    public ParcelUsageController(RioDbContext dbContext, ILogger<ParcelUsageController> logger, KeystoneService keystoneService,
        IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
    {
    }

    [HttpPost("parcel-usages/csv/headers")]
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

    [HttpPost("geography/{geographyID}/parcel-usages/csv")]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    public async Task<IActionResult> NewCSVUpload([FromForm] ParcelUsageCsvUpsertDto parcelUsageCsvUpsertDto, [FromRoute] int geographyID)
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

        if (!ParseCsvUpload(fileData, parcelUsageCsvUpsertDto.apnColumnName, parcelUsageCsvUpsertDto.quantityColumnName, out var records))
        {
            return BadRequest(ModelState);
        }

        if (!ValidateCsvUploadData(records, geographyID))
        {
            return BadRequest(ModelState);
        }

        var effectiveDate = DateTime.Parse(parcelUsageCsvUpsertDto.EffectiveDate);
        var parcelUsageCsvResponseDto = ParcelUsages.CreateStagingRecordsFromCSV(_dbContext, records, effectiveDate);

        return Ok(parcelUsageCsvResponseDto);
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

    private bool ValidateCsvUploadData(List<ParcelTransactionCSV> records, int geographyID)
    {
        var isValid = true;

        // no null APNs
        var nullAPNsCount = records.Count(x => x.APN == "");
        if (nullAPNsCount > 0)
        {
            ModelState.AddModelError("UploadedFile",
                $"The uploaded file contains {nullAPNsCount} {(nullAPNsCount > 1 ? "rows" : "row")} specifying a value with no corresponding APN.");
            isValid = false;
        }

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