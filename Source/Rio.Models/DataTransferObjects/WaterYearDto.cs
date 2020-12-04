using System;

namespace Rio.Models.DataTransferObjects
{
    public class WaterYearDto
    {
        public int WaterYearID { get; set; }
        public int Year { get; set; }
        public DateTime? FinalizeDate { get; set; }
    }
}