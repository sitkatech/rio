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
        private OpenETService _openETService;

        public OpenETRetrieveFromBucketJob(ILogger<OpenETRetrieveFromBucketJob> logger,
            IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext,
            IOptions<RioConfiguration> rioConfiguration, OpenETService openETService) : base("Parcel Evapotranspiration Update Job", logger, webHostEnvironment,
            rioDbContext)
        {
            _rioConfiguration = rioConfiguration.Value;
            _openETService = openETService;
        }

        public override List<RunEnvironment> RunEnvironments => new List<RunEnvironment>
            {RunEnvironment.Development, RunEnvironment.Staging, RunEnvironment.Production};

        public const string JobName = "Update Parcel Evapotranspiration Data";

        protected override void RunJobImplementation()
        {
            if (!_rioConfiguration.AllowOpenETSync || !_openETService.IsOpenETAPIKeyValid())
            {
                return;
            }
            
            var inProgressSyncs = _rioDbContext.OpenETSyncHistory
                .Where(x => x.OpenETSyncResultTypeID == (int) OpenETSyncResultTypeEnum.InProgress).ToList();
            if (inProgressSyncs.Any())
            {
                var filesReadyForExport = _openETService.GetAllFilesReadyForExport();
                inProgressSyncs.ForEach(x =>
                {
                    _openETService.UpdateParcelMonthlyEvapotranspirationWithETData(x.OpenETSyncHistoryID, filesReadyForExport);
                });
            }
            
            //Fail any created syncs that have been in a created state for longer than 15 minutes
            var createdSyncs = _rioDbContext.OpenETSyncHistory
                .Where(x => x.OpenETSyncResultTypeID == (int) OpenETSyncResultTypeEnum.Created).ToList();
            if (createdSyncs.Any())
            {
                createdSyncs.ForEach(x =>
                {
                    if (DateTime.UtcNow.Subtract(x.CreateDate).Minutes > 15)
                    {
                        OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, x.OpenETSyncHistoryID,
                            OpenETSyncResultTypeEnum.Failed,
                            "Request never exited the Created state. Please try again.");
                    }
                });
            }

        }
    }

    public interface IOpenETRetrieveFromBucketJob
    {
        void RunJob(IJobCancellationToken token);
    }
}
