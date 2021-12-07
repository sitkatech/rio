using System;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rio.API.GeoSpatial;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ParcelController : SitkaController<ParcelController>
    {
        public ParcelController(RioDbContext dbContext, ILogger<ParcelController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpGet("parcels/getParcelsWithAllocationAndUsage/{year}")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<ParcelAllocationAndUsageDto>> GetParcelsWithAllocationAndUsageByYear([FromRoute] int year)
        {
            var parcelDtos = ParcelAllocationAndUsage.GetByYear(_dbContext, year);
            var parcelAllocationBreakdownForYear = ParcelLedgers.GetParcelAllocationBreakdownForYearAsDto(_dbContext, year);
            var parcelDtosWithAllocation = parcelDtos
                .GroupJoin(
                    parcelAllocationBreakdownForYear,
                    x => x.ParcelID,
                    y => y.ParcelID,
                    (x, y) => new
                    {
                        ParcelAllocationAndUsage = x,
                        ParcelAllocationBreakdown = y
                    })
                .SelectMany(
                    parcelAllocationUsageAndBreakdown =>
                        parcelAllocationUsageAndBreakdown.ParcelAllocationBreakdown.DefaultIfEmpty(),
                    (x, y) =>
                    {
                        x.ParcelAllocationAndUsage.Allocations = y?.Allocations;
                        return x.ParcelAllocationAndUsage;
                    });
            return Ok(parcelDtosWithAllocation);
        }

        [HttpGet("parcels/inactive")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<ParcelDto>> GetInactiveParcels()
        {
            var parcelDtos = Parcel.ListInactiveAsDto(_dbContext);
            return Ok(parcelDtos);
        }

        [HttpGet("parcels/{parcelID}")]
        [ParcelViewFeature]
        public ActionResult<ParcelDto> GetByParcelID([FromRoute] int parcelID)
        {
            var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

            if (currentUser == null)
            {
                return Forbid();
            }

            if (currentUser != null && 
                currentUser.Role.RoleID == (int)RoleEnum.LandOwner)
            {
                var currentYear = WaterYear.GetDefaultYearToDisplay(_dbContext);
                var parcelsForUser = Parcel.ListByUserID(_dbContext, currentUser.UserID, currentYear.Year);

                if (!parcelsForUser.Any() || parcelsForUser.All(x => x.ParcelID != parcelID))
                {
                    return Forbid();
                }
            }

            var parcelDto = Parcel.GetByIDAsDto(_dbContext, parcelID);
            return RequireNotNullThrowNotFound(parcelDto, "Parcel", parcelID);
        }

        [HttpGet("parcels/search/{parcelNumber}")]
        [ParcelViewFeature]
        public ActionResult<List<string>> SearchByParcelNumber([FromRoute] string parcelNumber)
        {
            var parcelNumbers = Parcel.SearchParcelNumber(_dbContext, parcelNumber);
            return Ok(parcelNumbers);
        }

        [HttpGet("parcels/{parcelID}/getLedgerEntries")]
        [ParcelViewFeature]
        public ActionResult<List<ParcelLedgerDto>> GetAllLedgerEntriesByParcelID([FromRoute] int parcelID)
        {
            var parcelLedgerDtos = ParcelLedgers.ListByParcelIDAsDto(_dbContext, parcelID);
            return Ok(parcelLedgerDtos);
        }

        [HttpPost("parcels/{userID}/bulkSetAnnualParcelAllocation")]
        [ParcelManageFeature]
        public ActionResult BulkSetAnnualParcelAllocation([FromRoute] int userID, [FromBody] ParcelAllocationUpsertDto parcelAllocationUpsertDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: This needs to reference ParcelLedger. This will be tackled by Rio 346.

            ParcelAllocationHistory.CreateParcelAllocationHistoryEntity(_dbContext, userID, parcelAllocationUpsertDto, null);

            return Ok(0);
        }

        [HttpPost("parcels/{waterYear}/{waterTypeID}/bulkSetAnnualParcelAllocationFileUpload")]
        public async Task<ActionResult> BulkSetAnnualParcelAllocationFileUpload([FromRoute] int waterYear,
            [FromRoute] int waterTypeID)
        {
            var fileResource = await HttpUtilities.MakeFileResourceFromHttpRequest(Request, _dbContext, HttpContext);
            var waterTypeDisplayName =
                _dbContext.WaterTypes.Single(x => x.WaterTypeID == waterTypeID).WaterTypeName;

            if (!ParseBulkSetAllocationUpload(fileResource, waterTypeDisplayName, out var records, out var badRequestFromUpload))
            {
                return badRequestFromUpload;
            }

            if (!ValidateBulkSetAllocationUpload(records, waterTypeDisplayName, out var badRequestFromValidation))
            {
                return badRequestFromValidation;
            }

            _dbContext.FileResources.Add(fileResource);
            _dbContext.SaveChanges();

            // TODO: This needs to reference ParcelLedger. This will be tackled by Rio 346.

            ParcelAllocationHistory.CreateParcelAllocationHistoryEntity(_dbContext,
                UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID, fileResource.FileResourceID, waterYear,
                waterTypeID, null);

            return Ok();
        }

        [HttpGet("parcels/getParcelAllocationHistory")]
        [ManagerDashboardFeature]
        public ActionResult<List<ParcelAllocationHistoryDto>> GetParcelAllocationHistory()
        {
            return Ok(ParcelAllocationHistory.GetParcelAllocationHistoryDtos(_dbContext).ToList().OrderByDescending(x => x.ParcelAllocationHistoryDate));
        }

        [HttpPost("parcels/getBoundingBox")]
        [ParcelViewFeature]
        public ActionResult<BoundingBoxDto> GetBoundingBoxByParcelIDs([FromBody] ParcelIDListDto parcelIDListDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var boundingBoxDto = Parcel.GetBoundingBoxByParcelIDs(_dbContext, parcelIDListDto.ParcelIDs);
            return Ok(boundingBoxDto);
        }


        [HttpGet("parcels/getParcelsWithLandOwners/{year}")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<ParcelDto>> GetParcelsWithLandOwners([FromRoute] int year)
        {
            var parcelDtos = Parcel.ListForWaterYearAsDto(_dbContext, year);
            return Ok(parcelDtos);
        }

        [HttpGet("parcels/{parcelID}/getOwnershipHistory")]
        [ParcelViewFeature]
        public ActionResult<IEnumerable<ParcelOwnershipDto>> GetOwnershipHistory([FromRoute] int parcelID)
        {
            var parcelOwnershipDtos = Parcel.GetOwnershipHistory(_dbContext, parcelID).ToList().OrderByDescending(x => x.WaterYear.Year);

            return Ok(parcelOwnershipDtos);
        }

        [HttpPost("parcels/{parcelID}/changeOwner")]
        [ParcelManageFeature]
        public ActionResult<IEnumerable<ParcelOwnershipDto>> ChangeOwner([FromRoute] int parcelID, [FromBody] ParcelChangeOwnerDto parcelChangeOwnerDto)
        {
            var parcelDto = Parcel.GetByIDAsDto(_dbContext, parcelID);
            if (ThrowNotFound(parcelDto, "Parcel", parcelID, out var actionResult))
            {
                return actionResult;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var waterYearsToUpdate = parcelChangeOwnerDto.ApplyToSubsequentYears
                ? WaterYear.GetSubsequentWaterYearsInclusive(_dbContext, parcelChangeOwnerDto.EffectiveWaterYearID).Select(x => x.WaterYearID)
                : new List<int> {parcelChangeOwnerDto.EffectiveWaterYearID};

            AccountParcelWaterYear.ChangeParcelOwnerForWaterYears(_dbContext, parcelChangeOwnerDto.ParcelID, waterYearsToUpdate, parcelChangeOwnerDto.AccountID);

            Parcel.UpdateParcelStatus(_dbContext, parcelChangeOwnerDto.ParcelID, parcelChangeOwnerDto.AccountID.HasValue ? (int)ParcelStatusEnum.Active : (int)ParcelStatusEnum.Inactive);

            //If the Parcel was in the AccountReconciliation table, we can remove it
            AccountReconciliation.DeleteByParcelID(_dbContext, parcelID);

            return Ok();
        }

        private bool ParseBulkSetAllocationUpload(FileResource fileResource, string parcelTypeDisplayName, out List<BulkSetAllocationCSV> records, out ActionResult badRequest)
        {
            try
            {
                using var memoryStream = new MemoryStream(fileResource.FileResourceData);
                using var reader = new StreamReader(memoryStream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                csvReader.Configuration.RegisterClassMap(new BulkSetAllocationCSVMap(_dbContext,
                    parcelTypeDisplayName));
                csvReader.Read();
                csvReader.ReadHeader();
                var headerNamesDuplicated =
                    csvReader.Context.HeaderRecord.Where(x => !String.IsNullOrWhiteSpace(x)).GroupBy(x => x).Where(x => x.Count() > 1).ToList();
                if (headerNamesDuplicated.Any())
                {
                    badRequest = BadRequest(new
                    {
                        validationMessage =
                            $"The following header names appear more than once: {string.Join(", ", headerNamesDuplicated.OrderBy(x => x.Key).Select(x => x.Key))}"
                    });
                    records = null;
                    return false;
                }

                records = csvReader.GetRecords<BulkSetAllocationCSV>().ToList();
            }
            catch (HeaderValidationException e)
            {
                var headerMessage = e.Message.Split('.')[0];
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"{headerMessage}. Please check that the column name is not missing or misspelled."
                });
                records = null;
                return false;
            }
            catch (CsvHelper.MissingFieldException e)
            {
                var headerMessage = e.Message.Split('.')[0];
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"{headerMessage}. Please check that the column name is not missing or misspelled."
                });
                records = null;
                return false;
            }
            catch
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                       "There was an error parsing the CSV. Please ensure the file was formatted correctly."
                });
                records = null;
                return false;
            }

            badRequest = null;
            return true;
        }

        private bool ValidateBulkSetAllocationUpload(List<BulkSetAllocationCSV> records, string waterTypeDisplayName, out ActionResult badRequest)
        {
            // no duplicate apns permitted
            var duplicateAPNs = records.GroupBy(x => x.APN).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();

            if (duplicateAPNs.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"The upload contained multiples rows with these APNs: {string.Join(", ", duplicateAPNs)}"
                });
                return false;
            }

            // all parcel numbers must match
            var allParcelNumbers = _dbContext.Parcels.Select(y => y.ParcelNumber);
            var unmatchedRecords = records.Where(x => !allParcelNumbers.Contains(x.APN)).ToList();

            if (unmatchedRecords.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"The upload contained these APNs which did not match any record in the system: {string.Join(", ", unmatchedRecords.Select(x => x.APN))}"
                });
                return false;
            }

            // no null allocation amounts
            var nullAllocationQuantities = records.Where(x => x.AllocationQuantity == null).ToList();
            if (nullAllocationQuantities.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"The following APNs had no {waterTypeDisplayName} Quantity entered: {string.Join(", ", nullAllocationQuantities.Select(x => x.APN))}"
                });
                return false;
            }

            badRequest = null;
            return true;
        }

        [HttpGet("/parcels/parcelGDBCommonMappingToParcelStagingColumn")]
        public ActionResult GetParcelGDBCommonMappingToParcelStagingColumn()
        {
            var result = ParcelLayerGDBCommonMappingToParcelStagingColumn.GetCommonMappings(_dbContext);
            return Ok(result);
        }

        [HttpPost("/parcels/uploadGDB")]
        [RequestSizeLimit(524288000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        public async Task<IActionResult> UploadGDBAndParseFeatureClasses([FromForm] IFormFile inputFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            byte[] inputFileContents;
            await using (var ms = new MemoryStream(4096))
            {
                await inputFile.CopyToAsync(ms);
                inputFileContents = ms.ToArray();
            }
            // save the gdb file contents to UploadedGdb so user doesn't have to wait for upload of file again
            var uploadedGdbID = UploadedGdb.CreateNew(_dbContext, inputFileContents);

            using var disposableTempFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");
            var gdbFile = disposableTempFile.FileInfo;
            System.IO.File.WriteAllBytes(gdbFile.FullName, inputFileContents);

            try
            {
                var featureClassInfos = OgrInfoCommandLineRunner.GetFeatureClassInfoFromFileGdb(
                    _rioConfiguration.OgrInfoExecutable,
                    gdbFile.FullName,
                    250000000, _logger, 1);
                var uploadParcelLayerInfoDto = new UploadParcelLayerInfoDto()
                {
                    UploadedGdbID = uploadedGdbID,
                    FeatureClasses = featureClassInfos
                };

                return Ok(uploadParcelLayerInfoDto);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException e)
            {
                _logger.LogError(e, e.Message);
                UploadedGdb.Delete(_dbContext, uploadedGdbID);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                UploadedGdb.Delete(_dbContext, uploadedGdbID);
                return BadRequest("Error reading GDB file!");
            }
        }

        [HttpPost("/parcels/previewGDBChanges")]
        public ActionResult<ParcelUpdateExpectedResultsDto> PreviewParcelLayerGDBChangesViaGeoJsonFeatureCollectionAndUploadToStaging([FromBody] ParcelLayerUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var waterYearDto = WaterYear.GetByWaterYearID(_dbContext, model.YearChangesToTakeEffect);

            if (waterYearDto == null)
            {
                return BadRequest("Invalid water year selected");
            }

            var gdbFileContents = UploadedGdb.GetUploadedGdbFileContents(_dbContext, model.UploadedGDBID);
            using var disposableTempFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");
            var gdbFile = disposableTempFile.FileInfo;
            System.IO.File.WriteAllBytes(gdbFile.FullName, gdbFileContents);
            try
            {
                var ogr2OgrCommandLineRunner = new Ogr2OgrCommandLineRunner(_rioConfiguration.Ogr2OgrExecutable,
                    Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId,
                    250000000, false);
                var columns = model.ColumnMappings.Select(
                        x =>
                            $"{x.MappedColumnName} as {x.RequiredColumnName}").ToList();
                var geoJson = ogr2OgrCommandLineRunner.ImportFileGdbToGeoJson(gdbFile.FullName,
                    model.ParcelLayerNameInGDB, columns, null, _logger, null, false);
                var featureCollection = GeoJsonHelpers.GetFeatureCollectionFromGeoJsonString(geoJson, 14);
                var expectedResults = ParcelUpdateStaging.AddFromFeatureCollection(_dbContext, featureCollection, _rioConfiguration.ValidParcelNumberRegexPattern, _rioConfiguration.ValidParcelNumberPatternAsStringForDisplay, waterYearDto);
                return Ok(expectedResults);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException e)
            {
                _logger.LogError(e.Message);

                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return BadRequest("Error generating preview of changes!");
            }
        }

        [HttpPost("/parcels/enactGDBChanges")]
        public ActionResult EnactGDBChanges([FromBody] int waterYearID)
        {
            var waterYearDto = WaterYear.GetByWaterYearID(_dbContext, waterYearID);
            
            if (waterYearDto == null)
            {
                return BadRequest(
                    "There was an error applying these changes to the selected Water Year. Please try again, and if the problem persists contact support.");
            }

            var currentWaterYearDto = WaterYear.GetDefaultYearToDisplay(_dbContext);

            if (currentWaterYearDto.Year - waterYearDto.Year > 1)
            {
                return BadRequest(
                    "Changes may only be applied to the current year or the previous year. Please update Water Year selection and try again.");
            }

            if (waterYearDto.Year != currentWaterYearDto.Year && currentWaterYearDto.ParcelLayerUpdateDate != null)
            {
                return BadRequest(
                    "Because changes have been applied to the current year previously, you may only select the current year to apply these changes to. Please update Water Year selection and try again.");
            }

            using var dbContextTransaction = _dbContext.Database.BeginTransaction();

            var expectedResults = ParcelUpdateStaging.GetExpectedResultsDto(_dbContext, waterYearDto);

            try
            {
                if (expectedResults.NumAccountsToBeInactivated > 0 || expectedResults.NumAccountsToBeCreated > 0)
                {
                    var currentDifferencesForAccounts =
                        _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccounts.Where(x =>
                            !x.WaterYearID.HasValue || x.WaterYearID == waterYearDto.WaterYearID);

                    var accountNamesToCreate = currentDifferencesForAccounts
                        .Where(
                            x =>
                                !x.AccountAlreadyExists.Value)
                        .Select(x => x.AccountName)
                        .ToList();

                    Account.BulkCreateWithListOfNames(_dbContext, _rioConfiguration.VerificationKeyChars,
                        accountNamesToCreate);

                    var accountNamesToInactivate = currentDifferencesForAccounts
                        .Where(x => (x.AccountAlreadyExists.Value &&
                                     !string.IsNullOrWhiteSpace(x.ExistingParcels) && string.IsNullOrWhiteSpace(x.UpdatedParcels)) || (!x.AccountAlreadyExists.Value && string.IsNullOrWhiteSpace(x.UpdatedParcels))).Select(x => x.AccountName).ToList();
                    Account.BulkInactivate(_dbContext, _dbContext.Accounts
                        .Where(x => accountNamesToInactivate.Contains(x.AccountName))
                        .ToList());
                }

                _dbContext.Database.ExecuteSqlRaw(
                    "EXECUTE dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry {0}", waterYearDto.WaterYearID);

                WaterYear.UpdateParcelLayerUpdateDateForID(_dbContext, waterYearID);

                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                dbContextTransaction.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessage = GenerateParcelUpdateCompletedEmail(_rioConfiguration.WEB_URL, waterYearDto, expectedResults, smtpClient);
            SitkaSmtpClientService.AddCcRecipientsToEmail(mailMessage,
                EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
            SendEmailMessage(smtpClient, mailMessage);

            return Ok();
        }

        private MailMessage GenerateParcelUpdateCompletedEmail(string rioUrl, WaterYearDto waterYear, ParcelUpdateExpectedResultsDto expectedResultsDto,
            SitkaSmtpClientService smtpClient)
        {
            var messageBody = $@"The parcel data in the {_rioConfiguration.PlatformLongName} was updated. 
The following changes to the parcel layer were made for year {waterYear.Year}: <br/><br/>

Number of Accounts Unchanged: {expectedResultsDto.NumAccountsUnchanged}<br/>
Number of Accounts To Be Created: {expectedResultsDto.NumAccountsToBeCreated}<br/>
Number of Accounts To Be Inactivated: {expectedResultsDto.NumParcelsToBeInactivated}<br/><br/>

Number of Parcels Unchanged: {expectedResultsDto.NumParcelsUnchanged} <br/>
Number of Parcels With Updated Geometries: {expectedResultsDto.NumParcelsUpdatedGeometries}<br/>
Number of Parcels Associated With New Account: {expectedResultsDto.NumParcelsAssociatedWithNewAccount}<br/>
Number of Parcels To Be Inactivated: {expectedResultsDto.NumParcelsToBeInactivated}<br/><br/>

Number of Duplicate Parcel Numbers Found: {expectedResultsDto.NumParcelsWithConflicts}<br/><br/>

The updated Parcel data should be sent to the OpenET team, so that any new or modified parcels will be included in the automated OpenET data synchronization.
{smtpClient.GetSupportNotificationEmailSignature()}";

            var mailMessage = new MailMessage
            {
                Subject = $"Parcel Layer Update Completed In {_rioConfiguration.PlatformLongName}",
                Body = $"Hello,<br /><br />{messageBody}",
            };

            mailMessage.To.Add(smtpClient.GetDefaultEmailFrom());
            return mailMessage;
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = smtpClient.GetDefaultEmailFrom();
            mailMessage.ReplyToList.Add(_rioConfiguration.LeadOrganizationEmail);
            smtpClient.Send(mailMessage);
        }

    }

    public class ParcelLayerUpdateDto
    {
        public string ParcelLayerNameInGDB { get; set; }
        public int UploadedGDBID { get; set; }
        public List<ParcelRequiredColumnAndMappingDto> ColumnMappings { get; set; }
        public int YearChangesToTakeEffect { get; set; }
    }

    public class ParcelRequiredColumnAndMappingDto
    {
        public string RequiredColumnName { get; set; }
        public string MappedColumnName { get; set; }
        public string CommonName { get; set; }
    }

    public sealed class BulkSetAllocationCSVMap : ClassMap<BulkSetAllocationCSV>
    {
        public BulkSetAllocationCSVMap()
        {
            Map(m => m.APN).Name("APN");
            Map(m => m.AllocationQuantity).Name("Allocation Quantity");
        }

        public BulkSetAllocationCSVMap(RioDbContext dbContext, string waterTypeDisplayName)
        {
            Map(m => m.APN).Name("APN");
            Map(m => m.AllocationQuantity).Name(waterTypeDisplayName + " Quantity");
        }
    }
}
