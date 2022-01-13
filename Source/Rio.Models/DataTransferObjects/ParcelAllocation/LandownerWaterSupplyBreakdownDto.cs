using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.LandownerWaterSupplyBreakdownDto
{
    public class LandownerWaterSupplyBreakdownDto
    {
        public int AccountID { get; set; }
        public Dictionary<int,decimal> WaterSupplyByWaterType { get; set; }
    }
}