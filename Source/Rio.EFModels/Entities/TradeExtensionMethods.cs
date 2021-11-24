using Rio.Models.DataTransferObjects.Offer;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public static partial class TradeExtensionMethods
    {
        public static TradeWithMostRecentOfferDto AsTradeWithMostRecentOfferDto(this Trade trade)
        {
            var mostRecentOffer = trade.Offers.OrderByDescending(x => x.OfferDate).First();
            var tradeWithMostRecentOfferDto = new TradeWithMostRecentOfferDto()
            {
                TradeID = trade.TradeID,
                TradeNumber = trade.TradeNumber,
                TradeStatus = trade.TradeStatus.AsDto(),
                CreateAccount = trade.CreateAccount.AsSimpleDto(),
                OfferCreateAccountUser = mostRecentOffer.CreateAccount.AccountUsers.Any() ? mostRecentOffer.CreateAccount.AccountUsers.First().User?.AsSimpleDto() : null,
                OfferStatus = mostRecentOffer.OfferStatus.AsDto(),
                Price = mostRecentOffer.Price,
                Quantity = mostRecentOffer.Quantity,
                OfferDate = mostRecentOffer.OfferDate,
                OfferCreateAccount = mostRecentOffer.CreateAccount.AsSimpleDto(),
                WaterTransferID = null
            };

            var waterTransfer = mostRecentOffer.WaterTransfers.SingleOrDefault();
            if (waterTransfer != null)
            {
                tradeWithMostRecentOfferDto.WaterTransferID = waterTransfer.WaterTransferID;
                var sellerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Selling);
                var buyerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Buying);

                tradeWithMostRecentOfferDto.BuyerRegistration = buyerRegistration.AsSimpleDto();
                tradeWithMostRecentOfferDto.SellerRegistration = sellerRegistration.AsSimpleDto();
            }

            if (trade.Posting.PostingTypeID == (int) PostingTypeEnum.OfferToSell)
            {
                tradeWithMostRecentOfferDto.OfferPostingTypeID = trade.Posting.CreateAccountID == mostRecentOffer.CreateAccountID ? (int) PostingTypeEnum.OfferToSell : (int)PostingTypeEnum.OfferToBuy;
            }
            else
            {
                tradeWithMostRecentOfferDto.OfferPostingTypeID = trade.Posting.CreateAccountID == mostRecentOffer.CreateAccountID ? (int)PostingTypeEnum.OfferToBuy : (int)PostingTypeEnum.OfferToSell;
            }

            if (trade.Posting.PostingTypeID == (int) PostingTypeEnum.OfferToSell)
            {
                tradeWithMostRecentOfferDto.TradePostingTypeID = (int) PostingTypeEnum.OfferToBuy;
                tradeWithMostRecentOfferDto.Buyer = trade.CreateAccount.AsSimpleDto();
                tradeWithMostRecentOfferDto.Seller = trade.Posting.CreateAccount.AsSimpleDto();
            }
            else
            {
                tradeWithMostRecentOfferDto.TradePostingTypeID = (int) PostingTypeEnum.OfferToSell;
                tradeWithMostRecentOfferDto.Buyer = trade.Posting.CreateAccount.AsSimpleDto();
                tradeWithMostRecentOfferDto.Seller = trade.CreateAccount.AsSimpleDto();
            }

            return tradeWithMostRecentOfferDto;
        }
    }
}