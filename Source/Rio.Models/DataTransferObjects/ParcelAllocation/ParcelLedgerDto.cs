using System;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerDto
    {
        public int ParcelID { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int TransactionTypeID { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionDescription { get; set; }
        public int ParcelLedgerID { get; set; }
        public int WaterYear => EffectiveDate.Year;
        public int WaterMonth => EffectiveDate.Month;
        public int? WaterTypeID { get; set; }

    }

    public class ParcelLedgerDisplayDto : ParcelLedgerDto
    {
        public string WaterTypeDisplayName { get; set; }
        public string TransactionTypeDisplayName { get; set; }
    }
}