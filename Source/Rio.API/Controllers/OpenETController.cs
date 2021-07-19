﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class OpenETController : SitkaController<OpenETController>
    {
        public OpenETController(RioDbContext dbContext, ILogger<OpenETController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }

        [HttpGet("openet/is-api-key-valid")]
        [ManagerDashboardFeature]
        public ActionResult<bool> IsAPIKeyValid()
        {
            var httpClient = OpenETGoogleBucketHelpers.GetOpenETClientWithAuthorization(_rioConfiguration.OpenETAPIKey);
            var openETRequestURL = $"{_rioConfiguration.OpenETAPIBaseUrl}/home/key_expiration";
            var response = httpClient.GetAsync(openETRequestURL).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogCritical("OpenET API Key is invalid");
                return false;
            }

            var responseObject = JsonConvert.DeserializeObject<OpenETTokenExpirationDate>(response.Content.ReadAsStringAsync().Result);

            if (responseObject == null || responseObject.ExpirationDate < DateTime.UtcNow)
            {
                _logger.LogCritical("OpenET API Key is invalid or expired");
                return false;
            }

            return true;
        }

        public class OpenETTokenExpirationDate
        {
            [JsonProperty("Expiration date")]
            public DateTime ExpirationDate { get; set; }
        }


        [HttpPost("openet-sync-history/trigger-openet-google-bucket-refresh")]
        [ContentManageFeature]
        public ActionResult TriggerOpenETRefreshAndRetrieveJob([FromBody] int waterYearMonthID)
        {
            var triggerResponse = OpenETGoogleBucketHelpers.TriggerOpenETGoogleBucketRefresh(_rioConfiguration, _dbContext, waterYearMonthID);
            if (!triggerResponse.IsSuccessStatusCode)
            {
                var ores = StatusCode((int)triggerResponse.StatusCode, triggerResponse.Content.ReadAsStringAsync().Result);
                return ores;
            }

            return Ok();
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
