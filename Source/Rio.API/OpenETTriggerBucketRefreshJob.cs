using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Rio.EFModels.Entities;
using System;
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

        public OpenETTriggerBucketRefreshJob(ILogger<OpenETTriggerBucketRefreshJob> logger,
            IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext,
            IOptions<RioConfiguration> rioConfiguration, IBackgroundJobClient backgroundJobClient) : base("Trigger OpenET Google Bucket Update Job", logger, webHostEnvironment,
            rioDbContext)
        {
            _rioConfiguration = rioConfiguration.Value;
            _backgroundJobClient = backgroundJobClient;
        }

        public override List<RunEnvironment> RunEnvironments => new List<RunEnvironment>
            {RunEnvironment.Development, RunEnvironment.Staging, RunEnvironment.Production};

        public const string JobName = "OpenET Trigger Google Bucket Update";

        protected override void RunJobImplementation()
        {
            var nonFinalizedWaterYears = _rioDbContext.WaterYear.Where(x => !x.FinalizeDate.HasValue);
            if (!nonFinalizedWaterYears.Any())
            {
                return;
            }

            nonFinalizedWaterYears.ToList().ForEach(x =>
                {
                    OpenETGoogleBucketHelpers.TriggerOpenETGoogleBucketRefresh(_rioConfiguration, _rioDbContext, x.Year);
                });
            //var startYear = DateUtilities.MinimumYear;
            //var startDate = new DateTime(startYear, 1, 1);
            //var endDate = DateTime.Now.AddDays(-1);

            //if (!OpenETGoogleBucketHelpers.RasterUpdatedSinceMinimumLastUpdatedDate(_rioConfiguration, _rioDbContext, startDate, endDate))
            //{
            //    return;
            //}

            //var endDateString = endDate.ToString("yyyy-MM-dd");
            //var startDateString = startDate.ToString("yyyy-MM-dd");

            //var fileSuffix = _rioConfiguration.LeadOrganizationShortName + "_Nightly";
            //OpenETGoogleBucketHelpers.TriggerOpenETGoogleBucketRefresh(_rioConfiguration, _rioDbContext, _backgroundJobClient, startDateString, endDateString, fileSuffix);
        }
    }

    public interface IOpenETTriggerBucketRefreshJob
    {
        void RunJob(IJobCancellationToken token);
        void RunJob(IJobCancellationToken token, string additionalArguments);
    }
}
