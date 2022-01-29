using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rio.API.Converters;
using Rio.EFModels.Entities;

namespace Rio.API
{
    public class CimisService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CimisService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CimisService(HttpClient httpClient, ILogger<CimisService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonSerializerOptions = CreateDefaultJSONSerializerOptions();
        }

        private async Task<TV> GetJsonFromCatalogImpl<TV>(string uri)
        {
            return await _httpClient.GetFromJsonAsync<TV>(uri, _jsonSerializerOptions);
        }

        public async Task<CimisPrecipitationResponse> GetPrecipitationData(string appKey, DateTime startDate, DateTime endDate)
        {
            var endDateString = endDate.ToString("yyyy-MM-dd");
            var startDateString = startDate.ToString("yyyy-MM-dd");
            var cimisPrecipitationResponse = await GetJsonFromCatalogImpl<CimisPrecipitationResponse>($"?dataItems=day-precip&targets=5&unitsOfMeasure=E&appKey={appKey}&startDate={startDateString}&endDate={endDateString}");
            return cimisPrecipitationResponse;
        }

        public List<CimisPrecipitationDatum> CreateCimisPrecipitationDatumsFromResponse(CimisPrecipitationResponse? cimisPrecipitationResponse)
        {
            if (cimisPrecipitationResponse == null)
            {
                throw new NullReferenceException("Cimis Precipitation Response is null!  Check API to see if we got proper response");
            }
            var cimisPrecipitationDatums = cimisPrecipitationResponse.Data.Providers.Single().Records
                .Select(FromCimisPrecipitationResponse).ToList();
            return cimisPrecipitationDatums;
        }

        private static CimisPrecipitationDatum FromCimisPrecipitationResponse(Record record)
        {
            return new CimisPrecipitationDatum
            {
                DateMeasured = record.Date,
                Precipitation = record.DayPrecip.Value ?? 0,
                LastUpdated = DateTime.UtcNow
            };
        }

        public static JsonSerializerOptions CreateDefaultJSONSerializerOptions()
        {
            var jsonSerializerOptions = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
            jsonSerializerOptions.Converters.Add(new DecimalConverter());
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            jsonSerializerOptions.Converters.Add(new NullableConverterFactory());
            jsonSerializerOptions.PropertyNameCaseInsensitive = false;
            jsonSerializerOptions.PropertyNamingPolicy = null;
            jsonSerializerOptions.WriteIndented = true;
            jsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            jsonSerializerOptions.IgnoreNullValues = false;
            return jsonSerializerOptions;
        }
    }
}