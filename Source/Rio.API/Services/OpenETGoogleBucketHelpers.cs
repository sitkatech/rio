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
using Newtonsoft.Json;
using Rio.EFModels.Entities;

namespace Rio.API.Services
{
    public static class OpenETGoogleBucketHelpers
    {
        public const string OpenETBucketURL =
            "https://storage.googleapis.com/openet_raster_api_storage/openet_timeseries_multi_output/testing_multi_mean_API_KEY.csv";

        public const string TriggerTimeSeriesURL =
            "http://3.228.142.200/timeseries_multipolygon?shapefile_fn=projects/openet/featureCollections/Use_Case_Data/Rosedale-RioBravoWSD/RRBWSD_2019parcels_wgs84&start_date=START_DATE&end_date=END_DATE&model=sims&vars=et&aggregation_type=mean&api_key=API_KEY&to_cloud=openet_raster_api_storage";

        public const string CheckRasterUpdatedDateURL =
            "http://3.228.142.200//raster_collection_metadata?geom=-120.30921936035156,36.99542364399086,-120.30887603759766,36.98143783973302,-120.2918815612793,36.982260605282676,-120.29170989990234,36.99556074698967,-120.30921936035156,36.99542364399086&start_date=START_DATE&end_date=END_DATE&model=sims&vars=et&api_key=API_KEY&prop=date_ingested";

        public static bool RasterUpdatedSinceMinimumLastUpdatedDate(RioConfiguration rioConfiguration,
            RioDbContext rioDbContext, DateTime startDate, DateTime endDate)
        {
            //This function assumes you've already checked for finalized dates. May be worth optimizing

            var lastUpdatedDatesForDateRange = rioDbContext.OpenETSyncWaterYearStatus
                .Where(x => x.WaterYear >= startDate.Year && x.WaterYear <= endDate.Year)
                .Select(x => x.LastUpdatedDate);

            if (!lastUpdatedDatesForDateRange.Any() || lastUpdatedDatesForDateRange.Any(x => x == null))
            {
                return true;
            }

            var minLastUpdatedDateForDateRange = lastUpdatedDatesForDateRange.Min();

            var openETRequestURL = CheckRasterUpdatedDateURL.Replace("API_KEY", rioConfiguration.OpenETAPIKey).Replace("START_DATE", startDate.ToString("yyyy-MM-dd")).Replace("END_DATE", endDate.ToString("yyyy-MM-dd"));

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            var response = httpClient.GetAsync(openETRequestURL).Result;

            //If we don't get a good response, let's just say that there needs to be an update to cover our bases
            if (!response.IsSuccessStatusCode)
            {
                return true;
            }

            var responseObject = JsonConvert.DeserializeObject<RasterMetadataDateIngested>(response.Content.ReadAsStringAsync().Result);

            if (string.IsNullOrEmpty(responseObject.DateIngested) ||
                !DateTime.TryParse(responseObject.DateIngested, out DateTime responseDate) ||
                responseDate > minLastUpdatedDateForDateRange)
            {
                return true;
            }

            return false;
        }

        public class RasterMetadataDateIngested
        {
            [JsonProperty("date_ingested")]
            public string DateIngested { get; set; }
        }


        public static HttpResponseMessage TriggerOpenETGoogleBucketRefresh(
            RioConfiguration rioConfiguration, RioDbContext rioDbContext, string startDate, string endDate)
        {
            var openETRequestURL = TriggerTimeSeriesURL.Replace("API_KEY", rioConfiguration.OpenETAPIKey).Replace("START_DATE", startDate).Replace("END_DATE", endDate);

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            var responseResult = httpClient.GetAsync(openETRequestURL).Result;

            if (responseResult.IsSuccessStatusCode)
            {
                OpenETSyncHistory.New(rioDbContext, startDate, endDate);
            }

            //Really all we care about is if the request  was successful, but if we identify down the line that we can do anything with the response then could get that and save it
            return responseResult;
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
                OpenETSyncHistory.UpdateAnyInProgress(rioDbContext, OpenETSyncResultTypeEnum.Failed);
                return;
            }

            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                var csvr = new CsvReader(reader, CultureInfo.CurrentCulture);
                var records = csvr.GetRecords<OpenETCSVFormat>();
                var finalizedWaterYears = rioDbContext.OpenETSyncWaterYearStatus
                    .Where(x => x.OpenETSyncStatusTypeID == (int) OpenETSyncStatusTypeEnum.Finalized).Select(x => x.WaterYear).ToList();
                updatedEvapotranspirationDtos =
                    records.Where(x => !finalizedWaterYears.Contains(x.Date.Year)).Distinct(new DistinctOpenETCSVFormatComparer()).Select(x => x.AsOpenETGoogleBucketResponseEvapotranspirationData()).ToList();
            }

            if (updatedEvapotranspirationDtos.Any())
            {
                rioDbContext.Database.ExecuteSqlRaw(
                    "TRUNCATE TABLE dbo.OpenETGoogleBucketResponseEvapotranspirationData");
                rioDbContext.OpenETGoogleBucketResponseEvapotranspirationData.AddRange(updatedEvapotranspirationDtos);
                rioDbContext.SaveChanges();
                rioDbContext.Database.ExecuteSqlRaw("EXECUTE dbo.pUpdateParcelMonthlyEvapotranspirationWithETData");

                var waterYearsUpdated = updatedEvapotranspirationDtos.Select(x => x.WaterYear).Distinct().ToList();
                OpenETSyncWaterYearStatus.UpdateUpdatedDateAndAddIfNecessary(rioDbContext, waterYearsUpdated);
                OpenETSyncHistory.UpdateAnyInProgress(rioDbContext, OpenETSyncResultTypeEnum.Succeeded);
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
