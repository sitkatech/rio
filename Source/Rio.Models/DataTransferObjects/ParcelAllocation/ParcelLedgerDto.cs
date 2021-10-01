using System;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerDto
    {
        public int ParcelID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int TransactionTypeID { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionDescription { get; set; }
        public int ParcelLedgerID { get; set; }
        public int WaterYear => TransactionDate.Year;
    }
}