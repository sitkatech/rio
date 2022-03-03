//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterYearMonth]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class WaterYearMonthDto
    {
        public int WaterYearMonthID { get; set; }
        public WaterYearDto WaterYear { get; set; }
        public int Month { get; set; }
        public DateTime? FinalizeDate { get; set; }
    }

    public partial class WaterYearMonthSimpleDto
    {
        public int WaterYearMonthID { get; set; }
        public int WaterYearID { get; set; }
        public int Month { get; set; }
        public DateTime? FinalizeDate { get; set; }
    }

}