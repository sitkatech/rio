using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelWaterSupplyAndUsageDto : ParcelDto
    {
        public decimal? TotalSupply { get; set; }
        public decimal? Purchased { get; set; }
        public decimal? Sold { get; set; }
        public decimal? UsageToDate { get; set; }
        public Dictionary<int, decimal> WaterSupplyByWaterType { get; set; }
    }
}