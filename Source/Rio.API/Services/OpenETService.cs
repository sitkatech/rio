﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rio.API.Controllers;
using Rio.API.Services.Telemetry;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Services
{
    public class OpenETService
    {
        protected readonly ILogger<OpenETService> _logger;
        protected readonly RioConfiguration _rioConfiguration;
        protected readonly RioDbContext _rioDbContext;

        public OpenETService(ILogger<OpenETService> logger, RioConfiguration rioConfiguration, RioDbContext rioDbContext)
        {
            _logger = logger;
            _rioConfiguration = rioConfiguration;
            _rioDbContext = rioDbContext;
        }

        public bool RasterUpdatedSinceMinimumLastUpdatedDate(int month, int year, OpenETSyncHistoryDto newSyncHistory)
        {
            var top = _rioConfiguration.OpenETRasterMetadataBoundingBoxTop;
            var bottom = _rioConfiguration.OpenETRasterMetadataBoundingBoxBottom;
            var left = _rioConfiguration.OpenETRasterMetadataBoundingBoxLeft;
            var right = _rioConfiguration.OpenETRasterMetadataBoundingBoxRight;
            var openETRequestURL =
                $"{_rioConfiguration.OpenETAPIBaseUrl}/{_rioConfiguration.OpenETRasterMetadataRoute}?geometry={top},{left},{top},{right},{bottom},{right},{bottom},{left}&start_date={new DateTime(year, month, 1):yyyy-MM-dd}&end_date={new DateTime(year, month, DateTime.DaysInMonth(year, month)):yyyy-MM-dd}&model=ensemble&variable=et&ref_et_source=cimis&provisional=true";

            var httpClient = GetOpenETClientWithAuthorization();
            try
            {
                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                   throw new OpenETException($"Call to {openETRequestURL} was unsuccessful. Status Code: {response.StatusCode} Message: {body}");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<RasterMetadataDateIngested>(body);

                if (string.IsNullOrEmpty(responseObject.DateIngested) ||
                    !DateTime.TryParse(responseObject.DateIngested, out DateTime responseDate))
                {
                    newSyncHistory = OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                        OpenETSyncResultTypeEnum.DataNotAvailable);
                    return false;
                }

                var openETSyncHistoriesThatHaventFailed = _rioDbContext.OpenETSyncHistory
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

                newSyncHistory = OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.NoNewData);
                return false;
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(_logger, LogLevel.Critical, ex, "Error when attempting to check raster metadata date ingested.");
                newSyncHistory = OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed, ex.Message);
                return false;
            }
        }

        public class RasterMetadataDateIngested
        {
            [JsonProperty("date_ingested")]
            public string DateIngested { get; set; }
        }

        public string[] GetAllFilesReadyForExport()
        {
            var openETRequestURL =
                $"{_rioConfiguration.OpenETAPIBaseUrl}/{_rioConfiguration.OpenETAllFilesReadyForExportRoute}";

            var httpClient = GetOpenETClientWithAuthorization();

            try
            {
                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new OpenETException(
                        $"Call to {openETRequestURL} was unsuccessful. Status code: ${response.StatusCode} Message: {body}");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<ExportAllFilesResponse>(body);
                return responseObject.TimeseriesFilesReadyForExport;
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(_logger, LogLevel.Critical, ex, "Error when attempting to get all files that are ready for export.");
                return null;
            }
        }

        public class ExportAllFilesResponse
        {
            [JsonProperty("timeseries")]
            public string[] TimeseriesFilesReadyForExport { get; set; }
        }


        public HttpResponseMessage TriggerOpenETGoogleBucketRefresh(int waterYearMonthID)
        {
            if (!_rioConfiguration.AllowOpenETSync)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Syncing with OpenET is not enabled at this time")
                };
            }

            if (!IsOpenETAPIKeyValid())
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.PreconditionFailed,
                    Content = new StringContent(
                        "OpenET API Key is invalid or expired. Support has been notified and will work to remedy the situation shortly")
                };
            }

            var waterYearMonthDto = WaterYearMonth.GetByWaterYearMonthID(_rioDbContext, waterYearMonthID);

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

            if (_rioDbContext.OpenETSyncHistory
                .Any(x => x.WaterYearMonthID == waterYearMonthID && x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.InProgress))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent($"Sync already in progress for {monthNameToDisplay} {year}")
                };
            }

            var newSyncHistory = OpenETSyncHistory.New(_rioDbContext, waterYearMonthID);

            if (!RasterUpdatedSinceMinimumLastUpdatedDate(month, year, newSyncHistory))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.UnprocessableEntity,
                    Content = new StringContent(
                        $"The sync for {monthNameToDisplay} {year} will not be completed for the following reason: {newSyncHistory.OpenETSyncResultType.OpenETSyncResultTypeDisplayName}.{(newSyncHistory.OpenETSyncResultType.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.Failed ? " Error Message:" + newSyncHistory.ErrorMessage : "")}")
                };
            }

            var openETRequestURL =
                $"{_rioConfiguration.OpenETAPIBaseUrl}/{_rioConfiguration.OpenETRasterTimeSeriesMultipolygonRoute}?shapefile_asset_id={_rioConfiguration.OpenETShapefilePath}&start_date={new DateTime(year, month, 1):yyyy-MM-dd}&end_date={new DateTime(year, month, DateTime.DaysInMonth(year, month)):yyyy-MM-dd}&model=ensemble&variable=et&units=english&output_date_format=standard&ref_et_source=cimis&filename_suffix={_rioConfiguration.LeadOrganizationShortName + "_" + month + "_" + year + "_public"}&include_columns={_rioConfiguration.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier}&provisional=true";

            try
            {
                var httpClient = GetOpenETClientWithAuthorization();

                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new OpenETException($"Call to {openETRequestURL} failed. Status Code: {response.StatusCode} Message: {body}");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<TimeseriesMultipolygonSuccessfulResponse>(body);

                OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.InProgress, null, responseObject.FileRetrievalURL);

                return response;
            }
            catch (Exception ex)
            {
                OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, newSyncHistory.OpenETSyncHistoryID,
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

            //One very unfortunate thing about OpenET's design is that they're forced to create a queue of requests and can't process multiple requests at once.
            //That, combined with no (at this moment 7/14/21) means of knowing whether or not a run has completed or failed other than checking to see if the file is ready for export means we have to implement some kind of terminal state.
            OpenETSyncHistory.UpdateOpenETSyncEntityByID(rioDbContext, syncHistory.OpenETSyncHistoryID, timeBetweenSyncCreationAndNow > 24 ? OpenETSyncResultTypeEnum.Failed : OpenETSyncResultTypeEnum.InProgress);
        }

        public void UpdateParcelMonthlyEvapotranspirationWithETData(int syncHistoryID, string[] filesReadyForExport)
        {
            var syncHistoryObject = OpenETSyncHistory.GetByOpenETSyncHistoryID(_rioDbContext, syncHistoryID);

            if (syncHistoryObject == null || syncHistoryObject.OpenETSyncResultType.OpenETSyncResultTypeID !=
                (int)OpenETSyncResultTypeEnum.InProgress)
            {
                //Bad request, we completed already and somehow were called again, or someone else decided we were done
                return;
            }

            if (String.IsNullOrWhiteSpace(syncHistoryObject.GoogleBucketFileRetrievalURL))
            {
                //We are somehow storing sync histories without file retrieval urls, this is not good
                TelemetryHelper.LogCaughtException(_logger, LogLevel.Critical, new OpenETException(
                    $"OpenETSyncHistory record:{syncHistoryObject.OpenETSyncHistoryID} was saved without a file retrieval URL but we attempted to update with it. Check integration!"), "Error communicating with OpenET API.");
                OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, syncHistoryObject.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Failed, "Record was saved with a Google Bucket File Retrieval URL. Support has been notified.");
                return;
            }

            if (!filesReadyForExport.Contains(syncHistoryObject.GoogleBucketFileRetrievalURL))
            {
                UpdateStatusAndFailIfOperationHasExceeded24Hours(_rioDbContext, syncHistoryObject);
                return;
            }

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };
            
            var response = httpClient.GetAsync(syncHistoryObject.GoogleBucketFileRetrievalURL).Result;

            if (!response.IsSuccessStatusCode)
            {
                UpdateStatusAndFailIfOperationHasExceeded24Hours(_rioDbContext, syncHistoryObject);
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
                    var finalizedWaterYearMonths = _rioDbContext.WaterYearMonth
                        .Where(x => x.FinalizeDate.HasValue)
                        .Select(x => new DateTime(x.WaterYear.Year, x.Month, 1))
                        .ToList();
                    csvr.Configuration.RegisterClassMap(
                        new OpenETCSVFormatMap(_rioConfiguration.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier));
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
                    OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, syncHistoryObject.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.NoNewData);
                    return;
                }

                _rioDbContext.Database.ExecuteSqlRaw(
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

                using (SqlConnection con = new SqlConnection(_rioConfiguration.DB_CONNECTION_STRING))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.OpenETGoogleBucketResponseEvapotranspirationData";
                        con.Open();
                        sqlBulkCopy.WriteToServer(table);
                        con.Close();
                    }
                }

                _rioDbContext.Database.ExecuteSqlRaw("EXECUTE dbo.pUpdateParcelMonthlyEvapotranspirationWithETData");

                OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, syncHistoryObject.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Succeeded);
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(_logger, LogLevel.Critical, ex, "Error parsing file from OpenET or getting records into database.");
                OpenETSyncHistory.UpdateOpenETSyncEntityByID(_rioDbContext, syncHistoryObject.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed, ex.Message);
            }
        }

        private HttpClient GetOpenETClientWithAuthorization()
        {
            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_rioConfiguration.OpenETAPIKey);

            return httpClient;
        }

        public bool IsOpenETAPIKeyValid()
        {
            var httpClient = GetOpenETClientWithAuthorization();
            var openETRequestURL = $"{_rioConfiguration.OpenETAPIBaseUrl}/home/key_expiration";
            try
            {
                var response = httpClient.GetAsync(openETRequestURL).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new OpenETException(
                        $"Call to {openETRequestURL} was unsuccessful. Status Code: {response.StatusCode} Message: {body}.");
                }

                var responseObject =
                    JsonConvert.DeserializeObject<OpenETController.OpenETTokenExpirationDate>(body);

                if (responseObject == null || responseObject.ExpirationDate < DateTime.UtcNow)
                {
                    throw new OpenETException($"Deserializing OpenET API Key validation response failed, or the key is expired. Expiration Date: {(responseObject?.ExpirationDate != null ? responseObject.ExpirationDate.ToString(CultureInfo.InvariantCulture) : "Not provided")}");
                }

                return true;
            }
            catch (Exception ex)
            {
                TelemetryHelper.LogCaughtException(_logger, LogLevel.Critical, ex, "Error validating OpenET API Key.");
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

    public class DistinctOpenETCSVFormatComparer : IEqualityComparer<OpenETCSVFormat>
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

    public class OpenETException : Exception
    {
        public OpenETException()
        {
        }

        public OpenETException(string message)
            : base(message)
        {
        }

        public OpenETException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
