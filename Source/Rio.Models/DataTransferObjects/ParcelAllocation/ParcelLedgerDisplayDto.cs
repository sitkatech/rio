namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerDisplayDto : ParcelLedgerDto
    {
        public string WaterTypeDisplayName { get; set; }
        public string TransactionTypeDisplayName { get; set; }
    }
}