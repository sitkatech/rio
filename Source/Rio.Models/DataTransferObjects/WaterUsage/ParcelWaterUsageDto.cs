using Newtonsoft.Json;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class ParcelWaterUsageDto
    {
        // this DTO is specifically for charting, so the object that represents a chartable datum needs to be reshaped for the charting library
        [JsonProperty("name")]
        public string ParcelNumber { get; set; }

        [JsonProperty("value")]
        public decimal WaterUsageInAcreFeet { get; set; }
    }
}