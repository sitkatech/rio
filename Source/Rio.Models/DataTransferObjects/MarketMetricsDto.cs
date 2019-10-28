using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.Models.DataTransferObjects
{
    public class MarketMetricsDto
    {
        public int? MostRecentOfferToBuyQuantity { get; set; }
        public decimal? MostRecentOfferToBuyPrice { get; set; }
        public int? MostRecentOfferToSellQuantity { get; set; }
        public decimal? MostRecentOfferToSellPrice { get; set; }
        public WaterTransferDto MostRecentWaterTransfer { get; set; }
        public int TotalBuyVolume { get; set; }
        public int TotalSellVolume { get; set; }
    }
}