using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class WaterUsageOverviewDto
    {
        public List<CumulativeWaterUsageByYearDto> Current { get; set; }
        public List<CumulativeWaterUsageByMonthDto> Historic { get; set; }
    }
}