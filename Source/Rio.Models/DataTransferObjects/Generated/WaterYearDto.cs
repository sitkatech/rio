//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterYear]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class WaterYearDto
    {
        public int WaterYearID { get; set; }
        public int Year { get; set; }
        public DateTime? ParcelLayerUpdateDate { get; set; }
    }

    public partial class WaterYearSimpleDto
    {
        public int WaterYearID { get; set; }
        public int Year { get; set; }
        public DateTime? ParcelLayerUpdateDate { get; set; }
    }

}