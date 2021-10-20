namespace Rio.Models.DataTransferObjects
{
    public class TransactionTypeDto
    {
        public int TransactionTypeID { get; set; }
        public string TransactionTypeName { get; set; }
        public bool IsAllocation { get; set; }
        public int SortOrder { get; set; }
    }
}