using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.Models.DataTransferObjects
{
    public class MarketMetricsDto
    {
        public OfferDto MostRecentOfferToBuy { get; set; }
        public OfferDto MostRecentOfferToSell { get; set; }
        public WaterTransferDto MostRecentWaterTransfer { get; set; }
        public int TotalBuyVolume { get; set; }
        public int TotalSellVolume { get; set; }
    }
}