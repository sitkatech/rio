using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("triggerOpenETRetrieveBackgroundJob")] public ActionResult TriggerOpenETRefreshAndRetrieveBackgroundJob()
        {
            RecurringJob.Trigger(OpenETTriggerBucketRefreshJob.JobName);
            return Ok();
        }

        [HttpPost("openet-sync-water-year-status/trigger-openet-google-bucket-refresh")]
        public ActionResult TriggerOpenETRefreshAndRetrieveJob([FromBody] int waterYear)
        {
            string startDate = new DateTime(waterYear, 1, 1).ToString("yyyy-MM-dd");
            string endDate = new DateTime(waterYear, 12, 31).ToString("yyyy-MM-dd");

            var triggerResponse =
                OpenETGoogleBucketHelpers.TriggerOpenETGoogleBucketRefresh(_rioConfiguration, _dbContext, startDate, endDate);

            if (!triggerResponse.IsSuccessStatusCode)
            {
                ObjectResult ores = StatusCode((int)triggerResponse.StatusCode, triggerResponse.Content.ReadAsStringAsync().Result);
                return ores;
            }

            _backgroundJobClient.Schedule<OpenETRetrieveFromBucketJob>(x => x.RunJob(null), TimeSpan.FromMinutes(15));

            return Ok();
        }

        [HttpGet("openet-sync-water-year-status")]
        [ManagerDashboardFeature]
        public ActionResult<OpenETSyncWaterYearStatusDto> List()
        {
            var response = OpenETSyncWaterYearStatus.List(_dbContext);
            return Ok(response);
        }

        [HttpPut("openet-sync-water-year-status/finalize")]
        public ActionResult<OpenETSyncWaterYearStatusDto> FinalizeOpenETSyncWaterYearStatus([FromBody] int openETSyncWaterYearStatusID)
        {
            var openETSyncWaterYearStatusDto = OpenETSyncWaterYearStatus.GetByOpenETSyncWaterYearStatusID(_dbContext, openETSyncWaterYearStatusID);
            if (openETSyncWaterYearStatusDto == null)
            {
                return NotFound();
            }

            var updatedOpenETSyncWaterYearStatusDto = OpenETSyncWaterYearStatus.Finalize(_dbContext, openETSyncWaterYearStatusID);
            return Ok(updatedOpenETSyncWaterYearStatusDto);
        }

        [HttpGet("openet-sync-history/current-in-progress")]
        public ActionResult<OpenETSyncHistoryDto> GetInProgressOpenSyncHistoryDto()
        {
            var inProgressDto = OpenETSyncHistory.GetInProgress(_dbContext);
            return Ok(inProgressDto);
        }
    }
}
