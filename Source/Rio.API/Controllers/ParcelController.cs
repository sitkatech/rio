using System;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;
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


        [HttpGet("parcels/getParcelsWithWaterSupplyAndUsage/{year}")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<ParcelWaterSupplyAndUsageDto>> GetParcelsWithWaterSupplyAndUsageByYear([FromRoute] int year)
        {
            var parcelWaterSupplyAndUsageDtos = ParcelWaterSupplyAndUsage.GetByYear(_dbContext, year).ToList();
            var parcelWaterSupplyBreakdownForYear = ParcelLedgers.GetParcelWaterSupplyBreakdownForYearAsDto(_dbContext, year);
            
            foreach (var parcelWaterSupplyAndUsageDto in parcelWaterSupplyAndUsageDtos)
            {
                var parcelWaterSupplyBreakdown = parcelWaterSupplyBreakdownForYear
                    .SingleOrDefault(x => x.ParcelID == parcelWaterSupplyAndUsageDto.ParcelID);
                parcelWaterSupplyAndUsageDto.WaterSupplyByWaterType = parcelWaterSupplyBreakdown?.WaterSupplyByWaterType;
            }

            return Ok(parcelWaterSupplyAndUsageDtos);
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
            if (!UserCanAccessParcel(_dbContext, currentUser, parcelID))
            {
                return Forbid();
            }

            var parcelDto = Parcel.GetByIDAsDto(_dbContext, parcelID);
            return RequireNotNullThrowNotFound(parcelDto, "Parcel", parcelID);
        }

        [HttpGet("parcels/{parcelID}/getWithTags")]
        [ParcelViewFeature]
        public ActionResult<ParcelDto> GetWithTagsByParcelID([FromRoute] int parcelID)
        {
            var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            if (!UserCanAccessParcel(_dbContext, currentUser, parcelID))
            {
                return Forbid();
            }

            var parcelDto = Parcel.GetByIDAsDto(_dbContext, parcelID);
            parcelDto.Tags = Tags.ListByParcelIDAsDto(_dbContext, parcelID);

            return RequireNotNullThrowNotFound(parcelDto, "Parcel", parcelID);
        }

        [HttpGet("parcels/{tagID}/listByTagID")]
        [ParcelViewFeature]
        public ActionResult<List<ParcelSimpleDto>> ListByTagID([FromRoute] int tagID)
        {
            var parcelSimpleDtos = Parcel.ListByTagIDAsSimpleDto(_dbContext, tagID);
            return Ok(parcelSimpleDtos);
        }

        private static bool UserCanAccessParcel(RioDbContext dbContext, UserDto user, int parcelID)
        {
            if (user == null)
            {
                return false;
            }

            if (user != null &&
                user.Role.RoleID == (int)RoleEnum.LandOwner)
            {
                var currentYear = WaterYear.GetDefaultYearToDisplay(dbContext);
                var parcelsForUser = Parcel.ListByUserID(dbContext, user.UserID, currentYear.Year);

                if (!parcelsForUser.Any() || parcelsForUser.All(x => x.ParcelID != parcelID))
                {
                    return false;
                }
            }

            return true;
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
}
