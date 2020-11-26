using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rio.EFModels.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rio.API.Services;

namespace Rio.API
{

    public class OpenETRetrieveFromBucketJob : ScheduledBackgroundJobBase<OpenETRetrieveFromBucketJob>,
        IOpenETRetrieveFromBucketJob
    {
        private readonly RioConfiguration _rioConfiguration;

        public const string OpenETBucketURL =
            "https://storage.googleapis.com/openet_raster_api_storage/openet_timeseries_multi_output/testing_multi_mean_API_KEY.csv";


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
            var openETRequestURL = OpenETBucketURL.Replace("API_KEY", _rioConfiguration.OpenETAPIKey);

            var httpClient = new HttpClient();

            List<OpenETGoogleBucketResponseEvapotranspirationData> updatedEvapotranspirationDtos;

            httpClient.Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond);
            var response = httpClient.GetAsync(openETRequestURL).Result;

            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                var csvr = new CsvReader(reader, CultureInfo.CurrentCulture);
                var records = csvr.GetRecords<OpenETCSVFormat>();
                updatedEvapotranspirationDtos =
                    records.Distinct(new DistinctOpenETCSVFormatComparer()).Select(x => x.AsOpenETGoogleBucketResponseEvapotranspirationData()).ToList();
            }

            if (updatedEvapotranspirationDtos.Any())
            {
                _rioDbContext.Database.ExecuteSqlRaw(
                    "TRUNCATE TABLE dbo.OpenETGoogleBucketResponseEvapotranspirationData");
                _rioDbContext.OpenETGoogleBucketResponseEvapotranspirationData.AddRange(updatedEvapotranspirationDtos);
                _rioDbContext.SaveChanges();
                _rioDbContext.Database.ExecuteSqlRaw("EXECUTE dbo.pUpdateParcelMonthlyEvapotranspirationWithETData");
            }
        }
    }

    public interface IOpenETRetrieveFromBucketJob
    {
        void RunJob(IJobCancellationToken token);
    }

    public class OpenETCSVFormat
    {
        [Name("system:index")]
        public string SystemIndex { get; set; }

        [Name("APN_LABEL")]
        public string ParcelNumber { get; set; }

        [Name("OBJECTID")]
        public int ObjectID { get; set; }

        [Name("date")]
        public DateTime Date { get; set; }

        [Name("mean")]
        public decimal EvapotranspirationRate { get; set; }

        [Name(".geo")]
        public string Geo { get; set; }
    }

    class DistinctOpenETCSVFormatComparer : IEqualityComparer<OpenETCSVFormat>
    {

        public bool Equals(OpenETCSVFormat x, OpenETCSVFormat y)
        {
            return x.ParcelNumber == y.ParcelNumber &&
                   x.Date == y.Date &&
                   x.EvapotranspirationRate == y.EvapotranspirationRate;
        }

        public int GetHashCode(OpenETCSVFormat obj)
        {
            return obj.ParcelNumber.GetHashCode() ^
                   obj.Date.GetHashCode() ^
                   obj.EvapotranspirationRate.GetHashCode();
        }
    }

    public static class OpenETCSVFormatExtensionMethods
    {
        public static OpenETGoogleBucketResponseEvapotranspirationData AsOpenETGoogleBucketResponseEvapotranspirationData(this OpenETCSVFormat openETCSVFormat)
        {
            return new OpenETGoogleBucketResponseEvapotranspirationData()
            {
                ParcelNumber = openETCSVFormat.ParcelNumber,
                WaterYear = openETCSVFormat.Date.Year,
                WaterMonth = openETCSVFormat.Date.Month,
                EvapotranspirationRate = openETCSVFormat.EvapotranspirationRate
            };
        }
    }
}
