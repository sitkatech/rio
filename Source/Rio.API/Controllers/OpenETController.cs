using System;
using System.Collections.Generic;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class OpenETController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<OpenETController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;
        private IBackgroundJobClient _backgroundJobClient;

        public OpenETController(RioDbContext dbContext, ILogger<OpenETController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration, IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfiguration.Value;
            _backgroundJobClient = backgroundJobClient;
        }

        //For testing purposes
        [HttpGet("triggerOpenETRefreshBackgroundJob")] public ActionResult TriggerOpenETRefreshAndRetrieveBackgroundJob()
        {
            RecurringJob.Trigger(OpenETTriggerBucketRefreshJob.JobName);
            return Ok();
        }

        //For testing purposes
        [HttpGet("triggerOpenETRetrieveBackgroundJob")]
        public ActionResult TriggerOpenETRetrieveBackgroundJob()
        {
            _backgroundJobClient.Schedule<OpenETRetrieveFromBucketJob>(x => x.RunJob(null), TimeSpan.FromMinutes(0));
            return Ok();
        }

        [HttpPost("openet-sync-water-year-status/trigger-openet-google-bucket-refresh")]
        [ContentManageFeature]
        public ActionResult TriggerOpenETRefreshAndRetrieveJob([FromBody] int waterYear)
        {
            var triggerResponse =
                OpenETGoogleBucketHelpers.TriggerOpenETGoogleBucketRefresh(_rioConfiguration, _dbContext, waterYear);

            if (!triggerResponse.IsSuccessStatusCode)
            {
                ObjectResult ores = StatusCode((int)triggerResponse.StatusCode, triggerResponse.Content.ReadAsStringAsync().Result);
                return ores;
            }

            return Ok();
        }

        //[HttpGet("openet-sync-water-year-status")]
        //[ManagerDashboardFeature]
        //public ActionResult<OpenETSyncWaterYearStatusDto> List()
        //{
        //    var response = OpenETSyncWaterYearStatus.List(_dbContext);
        //    return Ok(response);
        //}

        //[HttpPut("openet-sync-water-year-status/finalize")]
        //[ContentManageFeature]
        //public ActionResult<OpenETSyncWaterYearStatusDto> FinalizeOpenETSyncWaterYearStatus([FromBody] int openETSyncWaterYearStatusID)
        //{
        //    var openETSyncWaterYearStatusDto = OpenETSyncWaterYearStatus.GetByOpenETSyncWaterYearStatusID(_dbContext, openETSyncWaterYearStatusID);
        //    if (openETSyncWaterYearStatusDto == null)
        //    {
        //        return NotFound();
        //    }

        //    var updatedOpenETSyncWaterYearStatusDto = OpenETSyncWaterYearStatus.Finalize(_dbContext, openETSyncWaterYearStatusID);
        //    return Ok(updatedOpenETSyncWaterYearStatusDto);
        //}

        [HttpGet("openet-sync-history/current-in-progress")]
        [ManagerDashboardFeature]
        public ActionResult<List<OpenETSyncHistoryDto>> ListInProgressOpenSyncHistoryDtos()
        {
            var inProgressDtos = OpenETSyncHistory.ListInProgress(_dbContext);
            return Ok(inProgressDtos);
        }

        [HttpGet("openet-sync-history")]
        [ManagerDashboardFeature]
        public ActionResult<List<OpenETSyncHistoryDto>> List()
        {
            var inProgressDtos = OpenETSyncHistory.List(_dbContext);
            return Ok(inProgressDtos);
        }
    }
}
