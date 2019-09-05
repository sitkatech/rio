using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class WaterUsageByParcelDto
    {
        public int Year { get; set; }
        public List<MonthlyWaterUsageDto> WaterUsage { get; set; }
    }

    public class MonthlyWaterUsageDto
    {
        public string Month { get; set; }
        public List<ParcelWaterUsageDto> WaterUsageByParcel { get; set; }
    }

    public class ParcelWaterUsageDto
    {
        public string ParcelNumber { get; set; }
        public decimal WaterUsageInAcreFeet { get; set; }
    }

    public class CumulativeWaterUsageByYearDto
    {
        public int Year { get; set; }
        public List<CumulativeWaterUsageByMonthDto> CumulativeWaterUsage { get; set; }
    }

    public class WaterUsageOverviewDto
    {
        public List<CumulativeWaterUsageByYearDto> Current { get; set; }
        public List<CumulativeWaterUsageByMonthDto> Historic { get; set; }
    }


    public class CumulativeWaterUsageByMonthDto
    {
        [JsonProperty("name")]
        public string Month { get; set; }
        [JsonProperty("value")]
        public decimal CumulativeWaterUsageInAcreFeet { get; set; }
    }

    
}
