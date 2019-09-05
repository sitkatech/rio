using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class CumulativeWaterUsageByYearDto
    {
        public int Year { get; set; }
        public List<CumulativeWaterUsageByMonthDto> CumulativeWaterUsage { get; set; }
    }
}