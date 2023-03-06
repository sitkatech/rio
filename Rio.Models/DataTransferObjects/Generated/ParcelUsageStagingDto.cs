//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelUsageStaging]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelUsageStagingDto
    {
        public int ParcelUsageStagingID { get; set; }
        public ParcelDto Parcel { get; set; }
        public string ParcelNumber { get; set; }
        public DateTime ReportedDate { get; set; }
        public decimal ReportedValue { get; set; }
        public decimal? ReportedValueInAcreFeet { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public partial class ParcelUsageStagingSimpleDto
    {
        public int ParcelUsageStagingID { get; set; }
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public DateTime ReportedDate { get; set; }
        public decimal ReportedValue { get; set; }
        public decimal? ReportedValueInAcreFeet { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

}