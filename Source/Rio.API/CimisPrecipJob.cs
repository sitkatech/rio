using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rio.EFModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Rio.API.Services;

namespace Rio.API
{

    public class CimisPrecipJob : ScheduledBackgroundJobBase<CimisPrecipJob>, ICimisPrecipJob
    {
        public static HttpClient HttpClient { get; set; }

        // todo: break this up a little, including constifying the base URL, the items, the targets, and make the key into a config
        public const string CimisBaseUrl =
            "http://et.water.ca.gov/api/data?appKey=70d96ca9-efe3-4bed-b53c-f627145a3928&dataItems=day-precip&targets=5";

        static CimisPrecipJob()
        {
            HttpClient = new HttpClient();
        }

        public CimisPrecipJob(ILogger<CimisPrecipJob> logger, IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext) : base("Precipitation Update Job", logger, webHostEnvironment, rioDbContext)
        {
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
            
            int startYear = DateUtilities.MinimumYear;

            var endDate = DateTime.Now.AddDays(-1);
            var startDate = new DateTime(startYear, 1, 1);

            var endDateString = endDate.ToString("yyyy-MM-dd");
            var startDateString = startDate.ToString("yyyy-MM-dd");

            var cimisRequestUrl = CimisBaseUrl + $"&startDate={startDateString}&endDate={endDateString}";
            
            HttpClient.Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond);
            var responseContent = HttpClient.GetAsync(cimisRequestUrl).Result.Content.ReadAsStringAsync().Result;

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
