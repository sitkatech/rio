using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class MonthlyWaterUsageDto
    {
        public string Month { get; set; }
        public List<ParcelWaterUsageDto> WaterUsageByParcel { get; set; }
    }
}