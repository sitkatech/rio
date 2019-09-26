using System.Linq;
using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public static class OfferExtensionMethods
    {
        public static OfferDto AsDto(this Offer offer)
        {
            var offerDto = new OfferDto()
            {
                OfferID = offer.OfferID,
                OfferDate = offer.OfferDate,
                OfferNotes = offer.OfferNotes,
                Quantity = offer.Quantity,
                Price = offer.Price,
                CreateUser = offer.CreateUser.AsSimpleDto(),
                OfferStatus = offer.OfferStatus.AsDto(),
                TradeID = offer.TradeID,
                RegisteredBySeller = false,
                RegisteredByBuyer = false
            };
            var waterTransfer = offer.WaterTransfer.SingleOrDefault();
            if (waterTransfer != null)
            {
                var sellerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Selling);
                offerDto.RegisteredBySeller = sellerRegistration.DateRegistered.HasValue;
                offerDto.DateRegisteredBySeller = sellerRegistration.DateRegistered;
                var buyerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Buying);
                offerDto.RegisteredByBuyer = buyerRegistration.DateRegistered.HasValue;
                offerDto.DateRegisteredByBuyer = buyerRegistration.DateRegistered;
                offerDto.WaterTransferID = waterTransfer.WaterTransferID;
            }

            return offerDto;
        }
    }
}