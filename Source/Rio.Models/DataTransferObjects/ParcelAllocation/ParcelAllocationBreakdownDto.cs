using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelAllocationBreakdownDto
    {
        public int ParcelID { get; set; }
        public Dictionary<int, decimal> Allocations { get; set; }
    }
}