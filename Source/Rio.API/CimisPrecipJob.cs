using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Rio.EFModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rio.API.Services;

namespace Rio.API
{
    public class CimisPrecipJob : ScheduledBackgroundJobBase<CimisPrecipJob>, ICimisPrecipJob
    {
        private readonly CimisService _cimisService;
        private readonly RioConfiguration _rioConfiguration;


        public CimisPrecipJob(ILogger<CimisPrecipJob> logger, IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext, IOptions<RioConfiguration> rioConfiguration, CimisService cimisService) : base("Precipitation Update Job", logger, webHostEnvironment, rioDbContext)
        {
            _cimisService = cimisService;
            _rioConfiguration = rioConfiguration.Value;
        }

        public override List<RunEnvironment> RunEnvironments => new() { RunEnvironment.Development, RunEnvironment.Staging, RunEnvironment.Production };
        public const string JobName = "Update Precipitation Data";

        protected override void RunJobImplementation()
        {
            var transactionDate = DateTime.UtcNow;
            const int startYear = DateUtilities.MinimumYear;
            var endYear = transactionDate.Year;

            var appKey = _rioConfiguration.CimisAppKey;

            //MP 3/23 From what I can tell, the CIMIS API essentially falls asleep when it isn't used
            //this causes an issue on first call because we're paying the cost for waking it up and then
            //our results don't come back. For now, we can just wait 15 minutes to give it some time
            //and then hopefully when we try again it'll be awake and ready to serve up some data
            var warmupCimisPrecipitationResponse = _cimisService.GetPrecipitationData(appKey, new DateTime(startYear, 1, 1), new DateTime(startYear, 1, 2)).Result;
            Thread.Sleep(15 * 60 * 1000);

            var newCimisPrecipitationData = new List<CimisPrecipitationDatum>();
            var years = Enumerable.Range(startYear, endYear - startYear + 1);
            foreach (var year in years)
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = year == endYear ? transactionDate.AddDays(-1) : new DateTime(year, 12, 31);

                var cimisPrecipitationResponse = _cimisService.GetPrecipitationData(appKey, startDate, endDate).Result;

                var cimisPrecipitationDatums = _cimisService.CreateCimisPrecipitationDatumsFromResponse(cimisPrecipitationResponse);
                newCimisPrecipitationData.AddRange(cimisPrecipitationDatums);
            }

            _rioDbContext.Database.ExecuteSqlRaw($"TRUNCATE TABLE dbo.CimisPrecipitationDatum");
            _rioDbContext.CimisPrecipitationData.AddRange(newCimisPrecipitationData);
            _rioDbContext.SaveChanges();
            _rioDbContext.Database.ExecuteSqlRaw($"dbo.pPublishCimisPrecipitationDataToParcelLedger");
        }
    }

    public interface ICimisPrecipJob
    {
        void RunJob(IJobCancellationToken token);
    }
}
