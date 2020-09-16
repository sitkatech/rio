using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class LandownerAllocationBreakdownDto
    {
        public int AccountID { get; set; }
        public Dictionary<int,decimal> Allocations { get; set; }
    }
}