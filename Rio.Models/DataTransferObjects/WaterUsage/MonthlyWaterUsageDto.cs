using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class MonthlyWaterUsageDto
    {
        [JsonProperty("name")]
        public string Month { get; set; }
        [JsonProperty("series")]
        public List<ParcelWaterUsageDto> WaterUsageByParcel { get; set; }
    }
}