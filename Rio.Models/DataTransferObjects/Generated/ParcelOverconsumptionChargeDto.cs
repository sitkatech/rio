//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelOverconsumptionCharge]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelOverconsumptionChargeDto
    {
        public int ParcelOverconsumptionChargeID { get; set; }
        public ParcelDto Parcel { get; set; }
        public WaterYearDto WaterYear { get; set; }
        public decimal OverconsumptionAmount { get; set; }
        public decimal OverconsumptionCharge { get; set; }
    }

    public partial class ParcelOverconsumptionChargeSimpleDto
    {
        public int ParcelOverconsumptionChargeID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYearID { get; set; }
        public decimal OverconsumptionAmount { get; set; }
        public decimal OverconsumptionCharge { get; set; }
    }

}