using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rio.EFModels.Entities;

namespace Rio.API.Services
{
    public static class OpenETGoogleBucketHelpers
    {
        public const string OpenETBucketURL =
            "https://storage.googleapis.com/openet_raster_api_storage/openet_timeseries_multi_output/testing_multi_mean_API_KEY.csv";

        public const string TriggerTimeSeriesURL =
            "http://3.228.142.200/timeseries_multipolygon?shapefile_fn=projects/openet/featureCollections/Use_Case_Data/Rosedale-RioBravoWSD/RRBWSD_2019parcels_wgs84&start_date=START_DATE&end_date=END_DATE&model=sims&vars=et&aggregation_type=mean&api_key=API_KEY&to_cloud=openet_raster_api_storage";

        public static HttpResponseMessage TriggerOpenETGoogleBucketRefresh(
            RioConfiguration rioConfiguration, string startDate, string endDate)
        {
            var openETRequestURL = TriggerTimeSeriesURL.Replace("API_KEY", rioConfiguration.OpenETAPIKey).Replace("START_DATE", startDate).Replace("END_DATE", endDate);

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };
            return httpClient.GetAsync(openETRequestURL).Result;
        }

        public static void UpdateParcelMonthlyEvapotranspirationWithETData(RioDbContext rioDbContext, RioConfiguration rioConfiguration)
        {
            var openETRequestURL = OpenETBucketURL.Replace("API_KEY", rioConfiguration.OpenETAPIKey);
            
            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            List<OpenETGoogleBucketResponseEvapotranspirationData> updatedEvapotranspirationDtos;

            var response = httpClient.GetAsync(openETRequestURL).Result;

            //From Will Carrara:
            //We have a cronjob running to change the permission to public every five minutes, so if you get a AccessDeniedAccess error check that link again in five minutes. 
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                TimeSpan waitTime = new TimeSpan(0, 5, 0);
                Thread.Sleep(waitTime);
                response = httpClient.GetAsync(openETRequestURL).Result;
            }

            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                var csvr = new CsvReader(reader, CultureInfo.CurrentCulture);
                var records = csvr.GetRecords<OpenETCSVFormat>();
                updatedEvapotranspirationDtos =
                    records.Distinct(new DistinctOpenETCSVFormatComparer()).Select(x => x.AsOpenETGoogleBucketResponseEvapotranspirationData()).ToList();
            }

            if (updatedEvapotranspirationDtos.Any())
            {
                rioDbContext.Database.ExecuteSqlRaw(
                    "TRUNCATE TABLE dbo.OpenETGoogleBucketResponseEvapotranspirationData");
                rioDbContext.OpenETGoogleBucketResponseEvapotranspirationData.AddRange(updatedEvapotranspirationDtos);
                rioDbContext.SaveChanges();
                rioDbContext.Database.ExecuteSqlRaw("EXECUTE dbo.pUpdateParcelMonthlyEvapotranspirationWithETData");
            }
        }
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
