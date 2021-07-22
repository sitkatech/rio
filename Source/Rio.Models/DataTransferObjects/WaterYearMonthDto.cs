using System;

namespace Rio.Models.DataTransferObjects
{
    public class WaterYearMonthDto
    {
        public int WaterYearMonthID { get; set; }
        public WaterYearDto WaterYear { get; set; }
        public int Month { get; set; }
        public DateTime? FinalizeDate { get; set; }
    }
}