using Newtonsoft.Json;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class CumulativeWaterUsageByMonthDto
    {
        // this DTO is specifically for charting, so the object that represents a chartable datum needs to be reshaped for the charting library
        [JsonProperty("name")]
        public string Month { get; set; }
        [JsonProperty("value")]
        public decimal? CumulativeWaterUsageInAcreFeet { get; set; }
    }
}