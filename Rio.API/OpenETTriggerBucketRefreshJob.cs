using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Rio.EFModels.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Rio.API.Services;

namespace Rio.API
{

    public class OpenETTriggerBucketRefreshJob : ScheduledBackgroundJobBase<OpenETTriggerBucketRefreshJob>,
        IOpenETTriggerBucketRefreshJob
    {
        private readonly RioConfiguration _rioConfiguration;
        private IBackgroundJobClient _backgroundJobClient;
        private readonly IOpenETService _openETService;

        public OpenETTriggerBucketRefreshJob(ILogger<OpenETTriggerBucketRefreshJob> logger,
            IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext,
            IOptions<RioConfiguration> rioConfiguration, IBackgroundJobClient backgroundJobClient, IOpenETService openETService) : base(JobName, logger, webHostEnvironment,
            rioDbContext)
        {
            _rioConfiguration = rioConfiguration.Value;
            _backgroundJobClient = backgroundJobClient;
            _openETService = openETService;
        }

        //This job should only run on Production. We have a limited quota for requesting data from OpenET,
        //so we can allow people to do on the fly requests just to show usability but especially when it comes
        //to the first time a new platform is released we want to rely on backups from Prod rather than have QA
        //make nightly requests, confirm they are good, and then make Prod do the same thing essentially doubling
        //our requests
        public override List<RunEnvironment> RunEnvironments => new() { RunEnvironment.Production};

        public const string JobName = "OpenET Trigger Google Bucket Update";

        protected override void RunJobImplementation()
        {
            if (!_rioConfiguration.AllowOpenETSync || !_openETService.IsOpenETAPIKeyValid())
            {
                return;
            }

            var nonFinalizedWaterYearMonths = _rioDbContext.WaterYearMonths.Where(x => !x.FinalizeDate.HasValue);
            if (!nonFinalizedWaterYearMonths.Any())
            {
                return;
            }

            nonFinalizedWaterYearMonths.ToList().ForEach(x =>
                {
                    _openETService.TriggerOpenETGoogleBucketRefresh(x.WaterYearMonthID);
                });
        }
    }

    public interface IOpenETTriggerBucketRefreshJob
    {
        void RunJob(IJobCancellationToken token);
    }
}
