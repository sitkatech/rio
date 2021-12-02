﻿using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rio.EFModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Options;
using Rio.API.Services;

namespace Rio.API
{

    public class CimisPrecipJob : ScheduledBackgroundJobBase<CimisPrecipJob>, ICimisPrecipJob
    {
        private readonly RioConfiguration _rioConfiguration;

        public const string CimisBaseUrl =
            "http://et.water.ca.gov/api/data?dataItems=day-precip&targets=5&unitsOfMeasure=E";


        public CimisPrecipJob(ILogger<CimisPrecipJob> logger, IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext, IOptions<RioConfiguration> rioConfiguration) : base("Precipitation Update Job", logger, webHostEnvironment, rioDbContext)
        {
            _rioConfiguration = rioConfiguration.Value;
        }

        public override List<RunEnvironment> RunEnvironments => new() { RunEnvironment.Development, RunEnvironment.Staging, RunEnvironment.Production };
        public const string JobName = "Update Precipitation Data";

        protected override void RunJobImplementation()
        {
            var waterType = _rioDbContext.WaterTypes.SingleOrDefault(x => x.IsSourcedFromApi);
            if (waterType == null)
            {
                return;
            }
            var waterTypeID = waterType.WaterTypeID;
            
            var startYear = DateUtilities.MinimumYear;

            var endDate = DateTime.Now.AddDays(-1);
            var startDate = new DateTime(startYear, 1, 1);

            var endDateString = endDate.ToString("yyyy-MM-dd");
            var startDateString = startDate.ToString("yyyy-MM-dd");

            var appKey = _rioConfiguration.CimisAppKey;

            var cimisRequestUrl = CimisBaseUrl + $"&appKey={appKey}&startDate={startDateString}&endDate={endDateString}";

            var responseContent = (string) null;

            for (int i = 0; i < 2; i++)
            {
                var httpClient = new HttpClient();

                httpClient.Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond);
                responseContent = httpClient.GetAsync(cimisRequestUrl).Result.Content.ReadAsStringAsync().Result;

                //MP 3/23 From what I can tell, the CIMIS API essentially falls asleep when it isn't used
                //this causes an issue on first call because we're paying the cost for waking it up and then
                //our results don't come back. For now, we can just wait 15 minutes to give it some time
                //and then hopefully when we try again it'll be awake and ready to serve up some data
                if (i == 0)
                {
                    Thread.Sleep(15 * 60 * 1000);
                }
            }
            
            var cimisPrecipitationResponse = JsonConvert.DeserializeObject<CimisPrecipitationResponse>(responseContent);

            var precipitationGroupedByYear = cimisPrecipitationResponse.Data.Providers.Single().Records
                .GroupBy(x => x.Date.Year).ToList();

            var parcelLedgers = new List<ParcelLedger>();
            
            foreach (var precipitationDataForYear in precipitationGroupedByYear)
            {
                var year = precipitationDataForYear.Key;
                var totalPrecipitation = precipitationDataForYear.Sum(y => y.DayPrecip.Value.GetValueOrDefault());

                parcelLedgers.AddRange(_rioDbContext.Parcels.Select(x => new ParcelLedger()
                {
                    TransactionTypeID = (int) TransactionTypeEnum.Supply,
                    TransactionAmount = totalPrecipitation * (decimal)x.ParcelAreaInAcres / (decimal)12.0,
                    WaterTypeID = waterTypeID,
                    ParcelLedgerEntrySourceTypeID = (int) ParcelLedgerEntrySourceTypeEnum.CIMIS,
                    ParcelID = x.ParcelID,
                    EffectiveDate = new DateTime(year, 1, 1),
                    TransactionDate = DateTime.UtcNow,
                    TransactionDescription =
                        $"Allocation of precipitation for {year} has been deposited into this water account."
                }));

            }

            _rioDbContext.ParcelLedgers.RemoveRange(_rioDbContext.ParcelLedgers.Where(x => x.WaterTypeID == waterTypeID));
                _rioDbContext.ParcelLedgers.AddRange(parcelLedgers);
                _rioDbContext.SaveChanges();
        }
    }

    public interface ICimisPrecipJob
    {
        void RunJob(IJobCancellationToken token);
    }

    public class CimisPrecipitationResponse
    {

        public ResponseData Data { get; set; }

        public class ResponseData
        {
            public List<Provider> Providers { get; set; }
        }

        public class Provider
        {
            public List<Record> Records { get; set; }
        }

        public class Record
        {
            public DateTime Date { get; set; }
            public Precipitation DayPrecip { get; set; }
        }

        public class Precipitation
        {
            public decimal? Value { get; set; }
        }
    }
}
