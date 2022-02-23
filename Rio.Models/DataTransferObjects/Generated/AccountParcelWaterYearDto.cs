//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountParcelWaterYear]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class AccountParcelWaterYearDto
    {
        public int AccountParcelWaterYearID { get; set; }
        public AccountDto Account { get; set; }
        public ParcelDto Parcel { get; set; }
        public WaterYearDto WaterYear { get; set; }
    }

    public partial class AccountParcelWaterYearSimpleDto
    {
        public int AccountParcelWaterYearID { get; set; }
        public int AccountID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYearID { get; set; }
    }

}