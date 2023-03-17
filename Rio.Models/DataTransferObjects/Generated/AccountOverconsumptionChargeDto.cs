//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountOverconsumptionCharge]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class AccountOverconsumptionChargeDto
    {
        public int AccountOverconsumptionChargeID { get; set; }
        public AccountDto Account { get; set; }
        public WaterYearDto WaterYear { get; set; }
        public decimal OverconsumptionAmount { get; set; }
        public decimal OverconsumptionCharge { get; set; }
    }

    public partial class AccountOverconsumptionChargeSimpleDto
    {
        public int AccountOverconsumptionChargeID { get; set; }
        public int AccountID { get; set; }
        public int WaterYearID { get; set; }
        public decimal OverconsumptionAmount { get; set; }
        public decimal OverconsumptionCharge { get; set; }
    }

}