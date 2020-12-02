using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rio.EFModels.Entities;

namespace Rio.API.Services
{
    public static class OpenETGoogleBucketHelpers
    {
        public const string OpenETBucketURL =
            "https://storage.googleapis.com/openet_raster_api_storage/openet_timeseries_multi_output/testing_multi_mean_API_KEY_FILE_SUFFIX.csv";

        public const string TriggerTimeSeriesURL =
            "http://3.228.142.200/timeseries_multipolygon?shapefile_fn=SHAPEFILE_PATH&start_date=START_DATE&end_date=END_DATE&model=ensemble&vars=et&aggregation_type=mean&api_key=API_KEY&to_cloud=openet_raster_api_storage&suffix=FILE_SUFFIX&out_columns=ParcelID,ParcelAre0";

        public const string CheckRasterUpdatedDateURL =
            "http://3.228.142.200//raster_collection_metadata?geom=-120.30921936035156,36.99542364399086,-120.30887603759766,36.98143783973302,-120.2918815612793,36.982260605282676,-120.29170989990234,36.99556074698967,-120.30921936035156,36.99542364399086&start_date=START_DATE&end_date=END_DATE&model=ensemble&vars=et&api_key=API_KEY&prop=date_ingested";

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


        public static HttpResponseMessage TriggerOpenETGoogleBucketRefresh(RioConfiguration rioConfiguration,
            RioDbContext rioDbContext, IBackgroundJobClient backgroundJobClient, string startDate, string endDate,
            string suffix)
        {
            var openETRequestURL = TriggerTimeSeriesURL
                .Replace("API_KEY", rioConfiguration.OpenETAPIKey)
                .Replace("SHAPEFILE_PATH", rioConfiguration.OpenETShapefilePath)
                .Replace("START_DATE", startDate)
                .Replace("END_DATE", endDate)
                .Replace("FILE_SUFFIX", suffix);

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            var responseResult = httpClient.GetAsync(openETRequestURL).Result;

            if (responseResult.IsSuccessStatusCode)
            {
                var syncHistoryObject = OpenETSyncHistory.New(rioDbContext, startDate, endDate, suffix);
                backgroundJobClient.Schedule<OpenETRetrieveFromBucketJob>(x => x.RunJob(null, syncHistoryObject.OpenETSyncHistoryID.ToString()), TimeSpan.FromMinutes(15));
            }

            return responseResult;
        }

        public static void UpdateParcelMonthlyEvapotranspirationWithETData(RioDbContext rioDbContext, RioConfiguration rioConfiguration, string syncHistoryIDAsString)
        {
            var syncHistoryID = Int32.Parse(syncHistoryIDAsString);

            var syncHistoryObject = OpenETSyncHistory.GetByOpenETSyncHistoryID(rioDbContext, syncHistoryID);

            if (syncHistoryObject == null || syncHistoryObject.OpenETSyncResultType.OpenETSyncResultTypeID !=
                (int) OpenETSyncResultTypeEnum.InProgress)
            {
                //Bad request, we completed already and somehow were called again, or someone else decided we were done
                return;
            }

            var openETRequestURL = OpenETBucketURL
                .Replace("API_KEY", rioConfiguration.OpenETAPIKey)
                .Replace("FILE_SUFFIX", syncHistoryObject.UpdatedFileSuffix);
            
            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

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
                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Failed);
                return;
            }

            List<OpenETGoogleBucketResponseEvapotranspirationData> distinctRecords;
            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                var csvr = new CsvReader(reader, CultureInfo.CurrentCulture);
                var finalizedWaterYears = rioDbContext.OpenETSyncWaterYearStatus
                    .Where(x => x.OpenETSyncStatusTypeID == (int)OpenETSyncStatusTypeEnum.Finalized)
                    .Select(x => x.WaterYear)
                    .ToList();
                distinctRecords = csvr.GetRecords<OpenETCSVFormat>().Where(x => !finalizedWaterYears.Contains(x.Date.Year))
                    .Distinct(new DistinctOpenETCSVFormatComparer())
                    .Select(x => x.AsOpenETGoogleBucketResponseEvapotranspirationData())
                    .ToList(); ;
            }

            if (distinctRecords.Any())
            {
                rioDbContext.Database.ExecuteSqlRaw(
                    "TRUNCATE TABLE dbo.OpenETGoogleBucketResponseEvapotranspirationData");
                DataTable table = new DataTable();
                table.Columns.Add("OpenETGoogleBucketResponseEvapotranspirationDataID", typeof(int));
                table.Columns.Add("ParcelID", typeof(int));
                table.Columns.Add("WaterMonth", typeof(int));
                table.Columns.Add("WaterYear", typeof(int));
                table.Columns.Add("EvapotranspirationRate", typeof(decimal));

                int i = 0;
                distinctRecords.ForEach(x =>
                    {
                        table.Rows.Add(++i, x.ParcelID, x.WaterMonth, x.WaterYear, x.EvapotranspirationRate);
                    });

                using (SqlConnection con = new SqlConnection(rioConfiguration.DB_CONNECTION_STRING))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.OpenETGoogleBucketResponseEvapotranspirationData";
                        con.Open();
                        sqlBulkCopy.WriteToServer(table);
                        con.Close();
                    }
                }

                rioDbContext.Database.ExecuteSqlRaw("EXECUTE dbo.pUpdateParcelMonthlyEvapotranspirationWithETData");

                var waterYearsUpdated = distinctRecords.Select(x => x.WaterYear).Distinct().ToList();
                OpenETSyncWaterYearStatus.UpdateUpdatedDateAndAddIfNecessary(rioDbContext, waterYearsUpdated);
                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Succeeded);
            }
        }
    }

    public class OpenETCSVFormat
    {
        [Name("system:index")]
        public string SystemIndex { get; set; }
        public int ParcelID { get; set; }

        [Name("ParcelAre0")] 
        public decimal ParcelAreaInAcres { get; set; }

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
            return x.ParcelID == y.ParcelID &&
                   x.Date == y.Date &&
                   x.EvapotranspirationRate == y.EvapotranspirationRate;
        }

        public int GetHashCode(OpenETCSVFormat obj)
        {
            return obj.ParcelID.GetHashCode() ^
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
                ParcelID = openETCSVFormat.ParcelID,
                WaterYear = openETCSVFormat.Date.Year,
                WaterMonth = openETCSVFormat.Date.Month,
                EvapotranspirationRate = ((openETCSVFormat.EvapotranspirationRate / (decimal)25.4) / 12) * openETCSVFormat.ParcelAreaInAcres
            };
        }
    }
}
