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
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Hangfire;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rio.API.Controllers;
using Rio.API.Services.Telemetry;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Serilog.Core;

namespace Rio.API.Services
{
    public static class OpenETGoogleBucketHelpers
    {

        public static bool RasterUpdatedSinceMinimumLastUpdatedDate<T>(RioConfiguration rioConfiguration,
            RioDbContext rioDbContext, int month, int year, OpenETSyncHistoryDto newSyncHistory, ILogger<T> logger)
        {
            var top = rioConfiguration.OpenETRasterMetadataBoundingBoxTop;
            var bottom = rioConfiguration.OpenETRasterMetadataBoundingBoxBottom;
            var left = rioConfiguration.OpenETRasterMetadataBoundingBoxLeft;
            var right = rioConfiguration.OpenETRasterMetadataBoundingBoxRight;
            var openETRequestURL =
                $"{rioConfiguration.OpenETAPIBaseUrl}/{rioConfiguration.OpenETRasterMetadataRoute}?geometry={top},{left},{top},{right},{bottom},{right},{bottom},{left}&start_date={new DateTime(year, month, 1):yyyy-MM-dd}&end_date={new DateTime(year, month, DateTime.DaysInMonth(year, month)):yyyy-MM-dd}&model=ensemble&variable=et&ref_et_source=cimis&provisional=true";

            var httpClient = GetOpenETClientWithAuthorization(rioConfiguration.OpenETAPIKey);
            try
            {

                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                        OpenETSyncResultTypeEnum.Failed, body);
                   throw new Exception($"Call to {openETRequestURL} was unsuccessful. Status Code: {response.StatusCode} Message: {body}");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<RasterMetadataDateIngested>(body);

                if (string.IsNullOrEmpty(responseObject.DateIngested) ||
                    !DateTime.TryParse(responseObject.DateIngested, out DateTime responseDate))
                {
                    OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                        OpenETSyncResultTypeEnum.DataNotAvailable);
                    return false;
                }

                var openETSyncHistoriesThatHaventFailed = rioDbContext.OpenETSyncHistory
                    .Include(x => x.WaterYearMonth)
                    .ThenInclude(x => x.WaterYear)
                    .Where(x => x.WaterYearMonth.WaterYear.Year == year && x.WaterYearMonth.Month == month &&
                                (x.OpenETSyncResultTypeID != (int) OpenETSyncResultTypeEnum.Failed) &&
                                x.OpenETSyncHistoryID != newSyncHistory.OpenETSyncHistoryID);

                if (!openETSyncHistoriesThatHaventFailed.Any())
                {
                    return true;
                }

                var mostRecentSyncHistory =
                    openETSyncHistoriesThatHaventFailed?.OrderByDescending(x => x.UpdateDate).First();

                if (responseDate > mostRecentSyncHistory.UpdateDate)
                {
                    return true;
                }

                OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.NoNewData);
                return false;
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(logger, LogLevel.Critical, ex, "Error when attempting to check raster metadata date ingested.");
                OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed, ex.Message);
                return false;
            }
        }

        public class RasterMetadataDateIngested
        {
            [JsonProperty("date_ingested")]
            public string DateIngested { get; set; }
        }

        public static string[] GetAllFilesReadyForExport<T>(RioConfiguration rioConfiguration, ILogger<T> logger)
        {
            var openETRequestURL =
                $"{rioConfiguration.OpenETAPIBaseUrl}/{rioConfiguration.OpenETAllFilesReadyForExportRoute}";

            var httpClient = GetOpenETClientWithAuthorization(rioConfiguration.OpenETAPIKey);

            try
            {
                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Call to {openETRequestURL} was unsuccessful. Status code: ${response.StatusCode} Message: {body}");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<ExportAllFilesResponse>(body);
                return responseObject.TimeseriesFilesReadyForExport;
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(logger, LogLevel.Critical, ex, "Error when attempting to get all files that are ready for export.");
                return null;
            }
        }

        public class ExportAllFilesResponse
        {
            [JsonProperty("timeseries")]
            public string[] TimeseriesFilesReadyForExport { get; set; }
        }


        public static HttpResponseMessage TriggerOpenETGoogleBucketRefresh<T>(RioConfiguration rioConfiguration,
            RioDbContext rioDbContext, int waterYearMonthID, ILogger<T> _logger)
        {
            if (!rioConfiguration.AllowOpenETSync)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Syncing with OpenET is not enabled at this time")
                };
            }

            if (!IsOpenETAPIKeyValid(rioConfiguration, _logger))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.PreconditionFailed,
                    Content = new StringContent(
                        "OpenET API Key is invalid or expired. Support has been notified and will work to remedy the situation shortly")
                };
            }

            var waterYearMonthDto = WaterYearMonth.GetByWaterYearMonthID(rioDbContext, waterYearMonthID);

            if (waterYearMonthDto == null)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Requested Water Year and Month not found")
                };
            }

            var year = waterYearMonthDto.WaterYear.Year;
            var month = waterYearMonthDto.Month;
            var monthNameToDisplay = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            if (rioDbContext.OpenETSyncHistory
                .Any(x => x.WaterYearMonthID == waterYearMonthID && x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.InProgress))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent($"Sync already in progress for {monthNameToDisplay} {year}")
                };
            }

            var newSyncHistory = OpenETSyncHistory.New(rioDbContext, waterYearMonthID);

            if (!RasterUpdatedSinceMinimumLastUpdatedDate(rioConfiguration, rioDbContext, month, year,
                newSyncHistory, _logger))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.UnprocessableEntity,
                    Content = new StringContent(
                        $"The sync for {monthNameToDisplay} {year} will not be completed for the following reason: {newSyncHistory.OpenETSyncResultType.OpenETSyncResultTypeDisplayName}.{(newSyncHistory.OpenETSyncResultType.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.Failed ? " Error Message:" + newSyncHistory.ErrorMessage : "")}")
                };
            }

            var openETRequestURL =
                $"{rioConfiguration.OpenETAPIBaseUrl}/{rioConfiguration.OpenETRasterTimeSeriesMultipolygonRoute}?shapefile_asset_id={rioConfiguration.OpenETShapefilePath}&start_date={new DateTime(year, month, 1):yyyy-MM-dd}&end_date={new DateTime(year, month, DateTime.DaysInMonth(year, month)):yyyy-MM-dd}&model=ensemble&variable=et&units=english&output_date_format=standard&ref_et_source=cimis&filename_suffix={rioConfiguration.LeadOrganizationShortName + "_" + month + "_" + year + "_public"}&include_columns={rioConfiguration.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier}&provisional=true";

            try
            {
                var httpClient = GetOpenETClientWithAuthorization(rioConfiguration.OpenETAPIKey);

                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Call to {openETRequestURL} failed. Status Code: {response.StatusCode} Message: {body}");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<TimeseriesMultipolygonSuccessfulResponse>(body);

                OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.InProgress, null, responseObject.FileRetrievalURL);

                return response;
            }
            catch (Exception ex)
            {
                OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed, ex.Message);
                TelemetryHelper.LogCaughtException(_logger, LogLevel.Critical, ex, "Error communicating with OpenET API.");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(
                        $"There was an error when attempting to create the request. Message: {ex.Message}")
                };
            }
        }

        public class TimeseriesMultipolygonSuccessfulResponse
        {
            [JsonProperty("bucket_url")]
            public string FileRetrievalURL { get; set; }
        }

        private static void UpdateStatusAndFailIfOperationHasExceeded24Hours(RioDbContext rioDbContext, OpenETSyncHistoryDto syncHistory)
        {
            var timeBetweenSyncCreationAndNow = DateTime.UtcNow.Subtract(syncHistory.CreateDate).Hours;

            //One very unfortunate thing about OpenET's design is that their forced to create a queue of requests and can't process multiple requests at once.
            //That, combined with no (at this moment 7/14/21) means of knowing whether or not
            OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, syncHistory.OpenETSyncHistoryID, timeBetweenSyncCreationAndNow > 24 ? OpenETSyncResultTypeEnum.Failed : OpenETSyncResultTypeEnum.InProgress);
        }

        public static void UpdateParcelMonthlyEvapotranspirationWithETData<T>(RioDbContext rioDbContext,
            RioConfiguration rioConfiguration, int syncHistoryID, string[] filesReadyForExport, ILogger<T> logger)
        {
            var syncHistoryObject = OpenETSyncHistory.GetByOpenETSyncHistoryID(rioDbContext, syncHistoryID);

            if (syncHistoryObject == null || syncHistoryObject.OpenETSyncResultType.OpenETSyncResultTypeID !=
                (int)OpenETSyncResultTypeEnum.InProgress)
            {
                //Bad request, we completed already and somehow were called again, or someone else decided we were done
                return;
            }

            if (String.IsNullOrWhiteSpace(syncHistoryObject.GoogleBucketFileRetrievalURL))
            {
                //We are somehow storing sync histories without file retrieval urls, this is not good
                TelemetryHelper.LogCaughtException(logger, LogLevel.Critical, new Exception(
                    $"OpenETSyncHistory record:{syncHistoryObject.OpenETSyncHistoryID} was saved without a file retrieval URL but we attempted to update with it. Check integration!"), "Error communicating with OpenET API.");
                OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Failed, "Record was saved with a Google Bucket File Retrieval URL. Support has been notified.");
                return;
            }

            if (!filesReadyForExport.Contains(syncHistoryObject.GoogleBucketFileRetrievalURL))
            {
                UpdateStatusAndFailIfOperationHasExceeded24Hours(rioDbContext, syncHistoryObject);
                return;
            }

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            var response = httpClient.GetAsync(syncHistoryObject.GoogleBucketFileRetrievalURL).Result;

            if (!response.IsSuccessStatusCode)
            {
                UpdateStatusAndFailIfOperationHasExceeded24Hours(rioDbContext, syncHistoryObject);
                return;
            }

            try
            {
                var fileContents = new MemoryStream();
                var gzipStream = new GZipInputStream(response.Content.ReadAsStreamAsync().Result);
                using (var tarInputStream = new TarInputStream(gzipStream, Encoding.UTF8))
                {
                    TarEntry entry;
                    while ((entry = tarInputStream.GetNextEntry()) != null)
                    {
                        if (entry.Name == syncHistoryObject.GoogleBucketFileRetrievalURL.Split('/').Last().Replace(".tar.gz", ".csv"))
                        {
                            tarInputStream.CopyEntryContents(fileContents);
                        }
                    }
                }

                fileContents.Position = 0;

                List<OpenETGoogleBucketResponseEvapotranspirationData> distinctRecords;
                using (var reader = new StreamReader(fileContents))
                {
                    var csvr = new CsvReader(reader, CultureInfo.CurrentCulture);
                    var finalizedWaterYearMonths = rioDbContext.WaterYearMonth
                        .Where(x => x.FinalizeDate.HasValue)
                        .Select(x => new DateTime(x.WaterYear.Year, x.Month, 1))
                        .ToList();
                    csvr.Configuration.RegisterClassMap(
                        new OpenETCSVFormatMap(rioConfiguration.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier));
                    //Sometimes the results will produce exact duplicates, so we need to filter those out
                    //Also one final check to make sure we don't get any finalized dates
                    distinctRecords = csvr.GetRecords<OpenETCSVFormat>().Where(x => !finalizedWaterYearMonths.Contains(x.Date))
                        .Distinct(new DistinctOpenETCSVFormatComparer())
                        .Select(x => x.AsOpenETGoogleBucketResponseEvapotranspirationData())
                        .ToList();
                }

                //This shouldn't happen, but if we enter here we've attempted to grab data for a water year that was finalized
                if (!distinctRecords.Any())
                {
                    OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.NoNewData);
                    return;
                }

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

                OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Succeeded);
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(logger, LogLevel.Critical, ex, "Error parsing file from OpenET or getting records into database.");
                OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed, ex.Message);
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

        public static bool IsOpenETAPIKeyValid<T>(RioConfiguration _rioConfiguration, ILogger<T> logger)
        {
            var httpClient = OpenETGoogleBucketHelpers.GetOpenETClientWithAuthorization(_rioConfiguration.OpenETAPIKey);
            var openETRequestURL = $"{_rioConfiguration.OpenETAPIBaseUrl}/home/key_expiration";
            try
            {
                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Call to {openETRequestURL} was unsuccessful. Status Code: {response.StatusCode} Message: {body}.");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<OpenETController.OpenETTokenExpirationDate>(body);

                if (responseObject == null || responseObject.ExpirationDate < DateTime.UtcNow)
                {
                    throw new Exception($"Deserializing OpenET API Key validation response failed, or the key is expired. Expiration Date: {(responseObject?.ExpirationDate != null ? responseObject.ExpirationDate.ToString(CultureInfo.InvariantCulture) : "Not provided")}");
                }

                return true;
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(logger, LogLevel.Critical, ex, "Error validating OpenET API Key.");
                return false;
            }
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
            Map(m => m.EvapotranspirationRate).Name("et_mean");
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
