using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.WaterUsage
{
    public class WaterUsageByParcelDto
    {
        public int Year { get; set; }
        public List<MonthlyWaterUsageDto> WaterUsage { get; set; }
    }
}
