using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Rio.EFModels.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore;

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
            if (!_rioConfiguration.AllowOpenETSync)
            {
                return;
            }
            
            var inProgressSyncs = _rioDbContext.OpenETSyncHistory
                .Where(x => x.OpenETSyncResultTypeID == (int) OpenETSyncResultTypeEnum.InProgress).ToList();
            if (inProgressSyncs.Any())
            {
                var filesReadyForExport = OpenETGoogleBucketHelpers.GetAllFilesReadyForExport(_rioConfiguration);
                inProgressSyncs.ForEach(x =>
                {
                    OpenETGoogleBucketHelpers.UpdateParcelMonthlyEvapotranspirationWithETData(_rioDbContext,
                            _rioConfiguration, x.OpenETSyncHistoryID, filesReadyForExport);
                });
            }

        }
    }

    public interface IOpenETRetrieveFromBucketJob
    {
        void RunJob(IJobCancellationToken token);
    }
}
