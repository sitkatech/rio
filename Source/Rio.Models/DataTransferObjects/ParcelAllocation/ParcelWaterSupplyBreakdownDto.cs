using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.ParcelWaterSupplyBreakdownDto
{
    public class ParcelWaterSupplyBreakdownDto
    {
        public int ParcelID { get; set; }
        public Dictionary<int, decimal> WaterSupplyByWaterType { get; set; }
    }
}