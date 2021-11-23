//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelLedger]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelLedgerDto
    {
        public int ParcelLedgerID { get; set; }
        public ParcelDto Parcel { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public TransactionTypeDto TransactionType { get; set; }
        public decimal TransactionAmount { get; set; }
        public WaterTypeDto WaterType { get; set; }
        public string TransactionDescription { get; set; }
        public UserDto User { get; set; }
        public string UserComment { get; set; }
    }

    public partial class ParcelLedgerSimpleDto
    {
        public int ParcelLedgerID { get; set; }
        public int ParcelID { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int TransactionTypeID { get; set; }
        public decimal TransactionAmount { get; set; }
        public int? WaterTypeID { get; set; }
        public string TransactionDescription { get; set; }
        public int? UserID { get; set; }
        public string UserComment { get; set; }
    }

}