//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelAllocationHistory]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelAllocationHistoryDto
    {
        public int ParcelAllocationHistoryID { get; set; }
        public DateTime ParcelAllocationHistoryDate { get; set; }
        public int ParcelAllocationHistoryWaterYear { get; set; }
        public WaterTypeDto WaterType { get; set; }
        public UserDto User { get; set; }
        public FileResourceDto FileResource { get; set; }
        public decimal? ParcelAllocationHistoryValue { get; set; }
    }

    public partial class ParcelAllocationHistorySimpleDto
    {
        public int ParcelAllocationHistoryID { get; set; }
        public DateTime ParcelAllocationHistoryDate { get; set; }
        public int ParcelAllocationHistoryWaterYear { get; set; }
        public int WaterTypeID { get; set; }
        public int UserID { get; set; }
        public int? FileResourceID { get; set; }
        public decimal? ParcelAllocationHistoryValue { get; set; }
    }

}