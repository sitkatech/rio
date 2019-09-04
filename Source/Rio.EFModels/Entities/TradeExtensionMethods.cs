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
            var tradeWithMostRecentOfferDto = new TradeWithMostRecentOfferDto()
            {
                TradeID = trade.TradeID,
                TradeStatus = trade.TradeStatus.AsDto(),
                OfferStatus = mostRecentOffer.OfferStatus.AsDto(),
                Price = mostRecentOffer.Price,
                Quantity = mostRecentOffer.Quantity,
                OfferDate = mostRecentOffer.OfferDate,
                OfferCreateUser = mostRecentOffer.CreateUser.AsSimpleDto(),
                IsConfirmed = false,
                WaterTransferID = null
            };

            var waterTransfer = mostRecentOffer.WaterTransfer.SingleOrDefault();
            if (waterTransfer != null)
            {
                tradeWithMostRecentOfferDto.WaterTransferID = waterTransfer.WaterTransferID;
                tradeWithMostRecentOfferDto.IsConfirmed = waterTransfer.ConfirmedByTransferringUser && waterTransfer.ConfirmedByReceivingUser;
            }

            if (trade.Posting.PostingTypeID == (int) PostingTypeEnum.OfferToSell)
            {
                tradeWithMostRecentOfferDto.OfferPostingTypeID = trade.Posting.CreateUserID == mostRecentOffer.CreateUserID ? (int) PostingTypeEnum.OfferToSell : (int)PostingTypeEnum.OfferToBuy;
            }
            else
            {
                tradeWithMostRecentOfferDto.OfferPostingTypeID = trade.Posting.CreateUserID == mostRecentOffer.CreateUserID ? (int)PostingTypeEnum.OfferToBuy : (int)PostingTypeEnum.OfferToSell;
            }

            if (trade.Posting.PostingTypeID == (int) PostingTypeEnum.OfferToSell)
            {
                tradeWithMostRecentOfferDto.TradePostingTypeID = (int) PostingTypeEnum.OfferToBuy;
                tradeWithMostRecentOfferDto.Buyer = trade.CreateUser.AsSimpleDto();
                tradeWithMostRecentOfferDto.Seller = trade.Posting.CreateUser.AsSimpleDto();
            }
            else
            {
                tradeWithMostRecentOfferDto.TradePostingTypeID = (int) PostingTypeEnum.OfferToSell;
                tradeWithMostRecentOfferDto.Buyer = trade.Posting.CreateUser.AsSimpleDto();
                tradeWithMostRecentOfferDto.Seller = trade.CreateUser.AsSimpleDto();
            }

            return tradeWithMostRecentOfferDto;
        }
    }
}