using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rio.EFModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Rio.API.Services;

namespace Rio.API
{

    public class CimisPrecipJob : ScheduledBackgroundJobBase<CimisPrecipJob>, ICimisPrecipJob
    {
        private readonly RioConfiguration _rioConfiguration;

        public const string CimisBaseUrl =
            "http://et.water.ca.gov/api/data?dataItems=day-precip&targets=5";


        public CimisPrecipJob(ILogger<CimisPrecipJob> logger, IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext, IOptions<RioConfiguration> rioConfiguration) : base("Precipitation Update Job", logger, webHostEnvironment, rioDbContext)
        {
            _rioConfiguration = rioConfiguration.Value;
        }

        public override List<RunEnvironment> RunEnvironments => new List<RunEnvironment> { RunEnvironment.Development, RunEnvironment.Staging, RunEnvironment.Production };
        public const string JobName = "Update Precipitation Data";

        protected override void RunJobImplementation()
        {
            var parcelAllocationType = _rioDbContext.ParcelAllocationType.SingleOrDefault(x => x.IsSourcedFromApi);
            if (parcelAllocationType == null)
            {
                return;
            }
            var parcelAllocationTypeID = parcelAllocationType.ParcelAllocationTypeID;
            
            var startYear = DateUtilities.MinimumYear;

            var endDate = DateTime.Now.AddDays(-1);
            var startDate = new DateTime(startYear, 1, 1);

            var endDateString = endDate.ToString("yyyy-MM-dd");
            var startDateString = startDate.ToString("yyyy-MM-dd");

            var appKey = _rioConfiguration.CimisAppKey;

            var cimisRequestUrl = CimisBaseUrl + $"&appKey={appKey}&startDate={startDateString}&endDate={endDateString}";
            
            var httpClient = new HttpClient();

            httpClient.Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond);
            var responseContent = httpClient.GetAsync(cimisRequestUrl).Result.Content.ReadAsStringAsync().Result;

            var cimisPrecipitationResponse = JsonConvert.DeserializeObject<CimisPrecipitationResponse>(responseContent);

            var precipitationGroupedByYear = cimisPrecipitationResponse.Data.Providers.Single().Records
                .GroupBy(x => x.Date.Year).ToList();

            List<ParcelAllocation> parcelAllocations = new List<ParcelAllocation>();
            
            foreach (var precipitationDataForYear in precipitationGroupedByYear)
            {
                var year = precipitationDataForYear.Key;
                var totalPrecipitation = precipitationDataForYear.Sum(y => y.DayPrecip.Value.GetValueOrDefault());

                parcelAllocations.AddRange(_rioDbContext.Parcel.Select(x => new ParcelAllocation()
                {
                    AcreFeetAllocated = totalPrecipitation * (decimal)x.ParcelAreaInAcres / (decimal)12.0,
                    ParcelAllocationTypeID = parcelAllocationTypeID,
                    ParcelID = x.ParcelID,
                    WaterYear = year
                }));

            }

            _rioDbContext.ParcelAllocation.RemoveRange(_rioDbContext.ParcelAllocation.Where(x => x.ParcelAllocationTypeID == parcelAllocationTypeID));
                _rioDbContext.ParcelAllocation.AddRange(parcelAllocations);
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
