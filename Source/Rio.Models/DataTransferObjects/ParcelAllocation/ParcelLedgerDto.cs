using System;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerDto
    {
        public int ParcelID { get; set; }
        public string? ParcelNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public TransactionTypeDto TransactionType { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionDescription { get; set; }
        public int ParcelLedgerID { get; set; }
        public int WaterYear => EffectiveDate.Year;
        public int WaterMonth => EffectiveDate.Month;
        public WaterTypeDto? WaterType { get; set; }
        public string? UserComment { get; set; }
    }
}