using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelAllocationAndUsageDto : ParcelDto
    {
        public decimal? Allocation { get; set; }
        public decimal? ProjectWater { get; set; }
        public decimal? Reconciliation { get; set; }
        public decimal? NativeYield { get; set; }
        public decimal? StoredWater { get; set; }
        public decimal? UsageToDate { get; set; }
        public Dictionary<int, decimal> Allocations { get; set; }
    }
}