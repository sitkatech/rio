using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects
{
    public class LandownerUsageReportDto
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public int AccountNumber { get; set; }
        public double? AcresManaged { get; set; }
        public decimal? Allocation { get; set; }
        public int? Purchased { get; set; }
        public int? Sold { get; set; }
        public decimal? TotalSupply { get; set; }
        public decimal? UsageToDate { get; set; }
        public decimal? CurrentAvailable { get; set; }
        public int NumberOfPostings { get; set; }
        public int NumberOfTrades { get; set; }
        public string MostRecentTradeNumber { get; set; }
        public Dictionary<int, decimal> Allocations { get; set; }
    }
}