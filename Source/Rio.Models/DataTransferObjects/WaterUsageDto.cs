using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects
{
    public class WaterUsageDto
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
}
