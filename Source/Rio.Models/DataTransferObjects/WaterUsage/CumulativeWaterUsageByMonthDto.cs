using Newtonsoft.Json;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class CumulativeWaterUsageByMonthDto
    {
        [JsonProperty("name")]
        public string Month { get; set; }
        [JsonProperty("value")]
        public decimal CumulativeWaterUsageInAcreFeet { get; set; }
    }
}