using System.Linq;
using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public static class TradeExtensionMethods
    {
        public static TradeDto AsDto(this Trade trade)
        {
            return new TradeDto()
            {
                TradeID = trade.TradeID,
                CreateUser = trade.CreateUser.AsSimpleDto(),
                TradeStatus = trade.TradeStatus.AsDto(),
                Posting = trade.Posting.AsDto()
            };
        }

        public static TradeWithMostRecentOfferDto AsTradeWithMostRecentOfferDto(this Trade trade)
        {
            var mostRecentOffer = trade.Offer.OrderByDescending(x => x.OfferDate).First();
            return new TradeWithMostRecentOfferDto()
            {
                TradeID = trade.TradeID,
                CreateUser = trade.CreateUser.AsSimpleDto(),
                TradeStatus = trade.TradeStatus.AsDto(),
                OfferStatus = mostRecentOffer.OfferStatus.AsDto(),
                Price = mostRecentOffer.Price,
                Quantity = mostRecentOffer.Quantity,
                OfferDate = mostRecentOffer.OfferDate
            };
        }
    }
}