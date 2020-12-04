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
using Rio.Models.DataTransferObjects;

namespace Rio.API.Services
{
    public static class OpenETGoogleBucketHelpers
    {

        public static bool RasterUpdatedSinceMinimumLastUpdatedDate(RioConfiguration rioConfiguration,
            RioDbContext rioDbContext, int year, OpenETSyncHistoryDto newSyncHistory)
        {
            var openETRequestURL =
                $"{rioConfiguration.OpenETCheckRasterUpdatedDateBaseURL}?geom=-120.30921936035156,36.99542364399086,-120.30887603759766,36.98143783973302,-120.2918815612793,36.982260605282676,-120.29170989990234,36.99556074698967,-120.30921936035156,36.99542364399086&api_key={rioConfiguration.OpenETAPIKey}&start_date={new DateTime(year, 1, 1):yyyy-MM-dd}&end_date={new DateTime(year, 12, 31):yyyy-MM-dd}&model=ensemble&vars=et&prop=date_ingested";

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
                .Include(x => x.WaterYear)
                .Any(x => x.WaterYear.Year == year && x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.InProgress))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Operation already in progress")
                };
            }

            var newSyncHistory = OpenETSyncHistory.New(rioDbContext, year);

            if (!RasterUpdatedSinceMinimumLastUpdatedDate(rioConfiguration, rioDbContext, year, newSyncHistory))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.UnprocessableEntity,
                    Content = new StringContent($"The request was successful, however the sync for {year} will not be completed for the following reason: {OpenETSyncHistory.GetByOpenETSyncHistoryID(rioDbContext, newSyncHistory.OpenETSyncHistoryID).OpenETSyncResultType.OpenETSyncResultTypeDisplayName}")
                };
            }

            var openETRequestURL =
                $"{rioConfiguration.OpenETTriggerTimeSeriesBaseURL}?shapefile_fn={rioConfiguration.OpenETShapefilePath}&start_date={new DateTime(year, 1, 1):yyyy-MM-dd}&end_date={new DateTime(year, 12, 31):yyyy-MM-dd}&model=ensemble&vars=et&aggregation_type=mean&api_key={rioConfiguration.OpenETAPIKey}&to_cloud=openet_raster_api_storage&suffix={newSyncHistory.GetFileSuffixForOpenETSyncHistoryDto(rioConfiguration.LeadOrganizationShortName)}&out_columns=ParcelNumb";

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            var responseResult = httpClient.GetAsync(openETRequestURL).Result;

            if (!responseResult.IsSuccessStatusCode)
            {
                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, newSyncHistory.OpenETSyncHistoryID,
                    OpenETSyncResultTypeEnum.Failed);
            }

            return responseResult;
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
                $"{rioConfiguration.OpenETGoogleBucketBaseURL}/testing_multi_mean_{rioConfiguration.OpenETAPIKey}_{syncHistoryObject.GetFileSuffixForOpenETSyncHistoryDto(rioConfiguration.LeadOrganizationShortName)}.csv";

            var httpClient = new HttpClient
            {
                Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond)
            };

            var response = httpClient.GetAsync(openETRequestURL).Result;

            if (!response.IsSuccessStatusCode)
            {
                var timeBetweenSyncCreationAndNow = DateTime.UtcNow.Subtract(syncHistoryObject.CreateDate).Hours;

                OpenETSyncHistory.UpdateSyncResultByID(rioDbContext, syncHistoryObject.OpenETSyncHistoryID, timeBetweenSyncCreationAndNow > 2 ? OpenETSyncResultTypeEnum.Failed : OpenETSyncResultTypeEnum.InProgress);

                return;
            }

            List<OpenETGoogleBucketResponseEvapotranspirationData> distinctRecords;
            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                var csvr = new CsvReader(reader, CultureInfo.CurrentCulture);
                var finalizedWaterYears = rioDbContext.WaterYear
                    .Where(x => x.FinalizeDate.HasValue)
                    .Select(x => x.Year)
                    .ToList();
                distinctRecords = csvr.GetRecords<OpenETCSVFormat>().Where(x => !finalizedWaterYears.Contains(x.Date.Year))
                    .Distinct(new DistinctOpenETCSVFormatComparer())
                    .Select(x => x.AsOpenETGoogleBucketResponseEvapotranspirationData())
                    .ToList(); ;
            }

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
                table.Columns.Add("EvapotranspirationRateInMM", typeof(decimal));

                int i = 0;
                distinctRecords.ForEach(x =>
                {
                    table.Rows.Add(++i, x.ParcelNumber, x.WaterMonth, x.WaterYear, x.EvapotranspirationRateInMM);
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
    }

    public class OpenETCSVFormat
    {
        [Name("system:index")]
        public string SystemIndex { get; set; }
        [Name("ParcelNumb")]
        public string ParcelNumber { get; set; }
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
                EvapotranspirationRateInMM = openETCSVFormat.EvapotranspirationRate
            };
        }
    }
}
