using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public static class TradeStatusExtensionMethods
    {
        public static TradeStatusDto AsDto(this TradeStatus tradeStatus)
        {
            return new TradeStatusDto()
            {
                TradeStatusID = tradeStatus.TradeStatusID,
                TradeStatusName = tradeStatus.TradeStatusName,
                TradeStatusDisplayName = tradeStatus.TradeStatusDisplayName
            };
        }
    }
}