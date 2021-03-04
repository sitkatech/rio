using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Hangfire;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Services
{
    public static class OpenETGoogleBucketHelpers
    {

        public static bool RasterUpdatedSinceMinimumLastUpdatedDate(RioConfiguration rioConfiguration,
            RioDbContext rioDbContext, int year, OpenETSyncHistoryDto newSyncHistory)
        {
            var top = rioConfiguration.OpenETRasterMetadataBoundingBoxTop;
            var bottom = rioConfiguration.OpenETRasterMetadataBoundingBoxBottom;
            var left = rioConfiguration.OpenETRasterMetadataBoundingBoxLeft;
            var right = rioConfiguration.OpenETRasterMetadataBoundingBoxRight;
            var openETRequestURL =
                $"{rioConfiguration.OpenETAPIBaseUrl}/{rioConfiguration.OpenETRasterMetadataRoute}?geometry={top},{left},{top},{right},{bottom},{right},{bottom},{left}&start_date={new DateTime(year, 1, 1):yyyy-MM-dd}&end_date={new DateTime(year, 12, 31):yyyy-MM-dd}&model=ensemble&variable=et&ref_et_source=cimis&provisional=true";

            var httpClient = GetOpenETClientWithAuthorization(rioConfiguration.OpenETAPIKey);

            var response = httpClient.GetAsync(openETRequestURL).Result;

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed);
                //Our API Key is bad
                throw new Exception("OpenET API Key has expired");
            }

            //If we don't get a good response, let's just say that there needs to be an update to cover our bases
            if (!response.IsSuccessStatusCode)
            {
                return true;
            }

            var responseObject = JsonConvert.DeserializeObject<RasterMetadataDateIngested>(response.Content.ReadAsStringAsync().Result);

            if (string.IsNullOrEmpty(responseObject.DateIngested) ||
                !DateTime.TryParse(responseObject.DateIngested, out DateTime responseDate))
            {
                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.DataNotAvailable);
                return false;
            }

            var openETSyncHistoriesThatHaventFailed = rioDbContext.OpenETSyncHistory
                .Include(x => x.WaterYear)
                .Where(x => x.WaterYear.Year == year &&
                            (x.OpenETSyncResultTypeID != (int)OpenETSyncResultTypeEnum.Failed) && x.OpenETSyncHistoryID != newSyncHistory.OpenETSyncHistoryID);

            if (!openETSyncHistoriesThatHaventFailed.Any())
            {
                return true;
            }

            var mostRecentSyncHistory = openETSyncHistoriesThatHaventFailed?.OrderByDescending(x => x.UpdateDate).First();

            if (responseDate > mostRecentSyncHistory.UpdateDate)
            {
                return true;
            }

            OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.NoNewData);
            return false;
        }

        public class RasterMetadataDateIngested
        {
            [JsonProperty("date_ingested")]
            public string DateIngested { get; set; }
        }


        public static HttpResponseMessage TriggerOpenETGoogleBucketRefresh(RioConfiguration rioConfiguration,
            RioDbContext rioDbContext, int waterYearID)
        {
            if (!rioConfiguration.AllowOpenETSync)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Syncing with OpenET is not enabled at this time")
                };
            }

            var waterYearDto = WaterYear.GetByWaterYearID(rioDbContext, waterYearID);

            if (waterYearDto == null)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Requested Water Year not found")
                };
            }

            var year = waterYearDto.Year;

            if (rioDbContext.OpenETSyncHistory
                .Any(x => x.WaterYearID == waterYearID && x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.InProgress))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent($"Sync already in progress for {year}")
                };
            }

            var newSyncHistory = OpenETSyncHistory.New(rioDbContext, waterYearID);

            if (!RasterUpdatedSinceMinimumLastUpdatedDate(rioConfiguration, rioDbContext, year, newSyncHistory))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.UnprocessableEntity,
                    Content = new StringContent($"The request was successful, however the sync for {year} will not be completed for the following reason: {OpenETSyncHistory.GetByOpenETSyncHistoryID(rioDbContext, newSyncHistory.OpenETSyncHistoryID).OpenETSyncResultType.OpenETSyncResultTypeDisplayName}")
                };
            }

            var openETRequestURL =
                $"{rioConfiguration.OpenETAPIBaseUrl}/{rioConfiguration.OpenETRasterTimeSeriesMultipolygonRoute}?shapefile_asset_id={rioConfiguration.OpenETShapefilePath}&start_date={new DateTime(year, 1, 1):yyyy-MM-dd}&end_date={new DateTime(year, 12, 31):yyyy-MM-dd}&model=ensemble&variable=et&units=in&ref_et_source=cimis&cloud_output_location=openet_raster_api_storage&filename_suffix={rioConfiguration.LeadOrganizationShortName + "_" + year}&include_columns={rioConfiguration.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier}&provisional=true";

            var httpClient = GetOpenETClientWithAuthorization(rioConfiguration.OpenETAPIKey);

            var response = httpClient.GetAsync(openETRequestURL).Result;

            var responseObject = JsonConvert.DeserializeObject<TimeseriesMultipolygonSuccessfulResponse>(response.Content.ReadAsStringAsync().Result);

            OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    response.IsSuccessStatusCode ? OpenETSyncResultTypeEnum.InProgress : OpenETSyncResultTypeEnum.Failed, responseObject.FileName, responseObject.TrackingNumber);

            return response;
        }

        public class TimeseriesMultipolygonSuccessfulResponse
        {
            [JsonProperty("filename")]
            public string FileName { get; set; }
            [JsonProperty("tracking_number")]
            public string TrackingNumber { get; set; }
        }

        public static void UpdateParcelMonthlyEvapotranspirationWithETData(RioDbContext rioDbContext, RioConfiguration rioConfiguration, int syncHistoryID)
        {
            var syncHistoryObject = OpenETSyncHistory.GetByOpenETSyncHistoryID(rioDbContext, syncHistoryID);

            if (syncHistoryObject == null || syncHistoryObject.OpenETSyncResultType.OpenETSyncResultTypeID !=
                (int)OpenETSyncResultTypeEnum.InProgress)
            {
                //Bad request, we completed already and somehow were called again, or someone else decided we were done
                return;
            }

            var openETRequestURL =
                $"{rioConfiguration.OpenETGoogleBucketBaseURL}/{syncHistoryObject.GoogleBucketFileSuffixForRetrieval}.tar.gz";

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            var response = httpClient.GetAsync(openETRequestURL).Result;

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID,
                        OpenETSyncResultTypeEnum.Failed);
                    //Our API Key is bad
                    throw new Exception("OpenET API Key has expired");
                }
                var timeBetweenSyncCreationAndNow = DateTime.UtcNow.Subtract(syncHistoryObject.CreateDate).Hours;

                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID, timeBetweenSyncCreationAndNow > 2 ? OpenETSyncResultTypeEnum.Failed : OpenETSyncResultTypeEnum.InProgress);

                return;
            }

            var fileContents = new MemoryStream();
            var gzipStream = new GZipInputStream(response.Content.ReadAsStreamAsync().Result);
            using (var tarInputStream = new TarInputStream(gzipStream, Encoding.UTF8))
            {
                TarEntry entry;
                while ((entry = tarInputStream.GetNextEntry()) != null)
                {
                    tarInputStream.CopyEntryContents(fileContents);
                }
            }

            fileContents.Position = 0;

            List<OpenETGoogleBucketResponseEvapotranspirationData> distinctRecords;
            using (var reader = new StreamReader(fileContents))
            {
                var csvr = new CsvReader(reader, CultureInfo.CurrentCulture);
                var finalizedWaterYears = rioDbContext.WaterYear
                    .Where(x => x.FinalizeDate.HasValue)
                    .Select(x => x.Year)
                    .ToList();
                csvr.Configuration.RegisterClassMap(
                    new OpenETCSVFormatMap(rioConfiguration.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier));
                //Sometimes the results will produce exact duplicates, so we need to filter those out
                //Also one final check to make sure we don't get any finalized dates
                distinctRecords = csvr.GetRecords<OpenETCSVFormat>().Where(x => !finalizedWaterYears.Contains(x.Date.Year))
                    .Distinct(new DistinctOpenETCSVFormatComparer())
                    .Select(x => x.AsOpenETGoogleBucketResponseEvapotranspirationData())
                    .ToList();
            }

            //This shouldn't happen, but if we enter here we've attempted to grab data for a water year that was finalized
            if (!distinctRecords.Any())
            {
                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.NoNewData);
                return;
            }

            try
            {
                rioDbContext.Database.ExecuteSqlRaw(
                    "TRUNCATE TABLE dbo.OpenETGoogleBucketResponseEvapotranspirationData");
                DataTable table = new DataTable();
                table.Columns.Add("OpenETGoogleBucketResponseEvapotranspirationDataID", typeof(int));
                table.Columns.Add("ParcelNumber", typeof(string));
                table.Columns.Add("WaterMonth", typeof(int));
                table.Columns.Add("WaterYear", typeof(int));
                table.Columns.Add("EvapotranspirationRateInches", typeof(decimal));

                var index = 0;
                distinctRecords.ForEach(x =>
                {
                    table.Rows.Add(++index, x.ParcelNumber, x.WaterMonth, x.WaterYear, x.EvapotranspirationRateInches);
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

                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Succeeded);
            }
            catch (Exception ex)
            {
                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed);
            }
        }

        public static HttpClient GetOpenETClientWithAuthorization(string apiKey)
        {
            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);

            return httpClient;
        }
    }

    public class OpenETCSVFormat
    {
        public string ParcelNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal EvapotranspirationRate { get; set; }
    }

    public class OpenETCSVFormatMap : ClassMap<OpenETCSVFormat>
    {
        public OpenETCSVFormatMap(string parcelNumberColumnName)
        {
            Map(m => m.ParcelNumber).Name(parcelNumberColumnName);
            Map(m => m.Date).Name("time");
            Map(m => m.EvapotranspirationRate).Name("mean");
        }
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
                EvapotranspirationRateInches = openETCSVFormat.EvapotranspirationRate
            };
        }
    }
}
