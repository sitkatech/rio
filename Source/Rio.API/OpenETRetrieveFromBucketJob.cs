using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Rio.EFModels.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Rio.API.Services;

namespace Rio.API
{

    public class OpenETRetrieveFromBucketJob : ScheduledBackgroundJobBase<OpenETRetrieveFromBucketJob>,
        IOpenETRetrieveFromBucketJob
    {
        private readonly RioConfiguration _rioConfiguration;

        public OpenETRetrieveFromBucketJob(ILogger<OpenETRetrieveFromBucketJob> logger,
            IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext,
            IOptions<RioConfiguration> rioConfiguration) : base("Parcel Evapotranspiration Update Job", logger, webHostEnvironment,
            rioDbContext)
        {
            _rioConfiguration = rioConfiguration.Value;
        }

        public override List<RunEnvironment> RunEnvironments => new List<RunEnvironment>
            {RunEnvironment.Development, RunEnvironment.Staging, RunEnvironment.Production};

        public const string JobName = "Update Parcel Evapotranspiration Data";

        protected override void RunJobImplementation()
        {
            RunJobImplementation(null);
        }

        protected override void RunJobImplementation(string additionalArguments)
        {
            OpenETGoogleBucketHelpers.UpdateParcelMonthlyEvapotranspirationWithETData(_rioDbContext, _rioConfiguration, additionalArguments);
        }
    }

    public interface IOpenETRetrieveFromBucketJob
    {
        void RunJob(IJobCancellationToken token);
        void RunJob(IJobCancellationToken token, string additionalArguments);
    }
}
