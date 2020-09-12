using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.BulkSetAllocationCSV
{
    public class ParcelAllocationBreakdown
    {
        public int ParcelID { get; set; }
        public Dictionary<string, decimal> Allocations { get; set; }
    }
}