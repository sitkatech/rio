namespace Rio.Models.DataTransferObjects
{
    public class LandownerUsageReportDto
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal? ProjectWater { get; set; }
        public decimal? Reconciliation { get; set; }
        public decimal? NativeYield { get; set; }
        public decimal? StoredWater { get; set; }
        public decimal? Allocation { get; set; }
        public int? Purchased { get; set; }
        public int? Sold { get; set; }
        public decimal? TotalSupply { get; set; }
        public decimal? UsageToDate { get; set; }
        public decimal? CurrentAvailable { get; set; }
        public int NumberOfPostings { get; set; }
        public int NumberOfTrades { get; set; }
        public string MostRecentTradeNumber { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}