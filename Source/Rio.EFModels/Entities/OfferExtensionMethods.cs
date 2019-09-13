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
                ConfirmedByTransferringUser = false,
                ConfirmedByReceivingUser = false
            };
            var waterTransfer = offer.WaterTransfer.SingleOrDefault();
            if (waterTransfer != null)
            {
                offerDto.ConfirmedByTransferringUser = waterTransfer.ConfirmedByTransferringUser;
                offerDto.DateConfirmedByTransferringUser = waterTransfer.DateConfirmedByTransferringUser;
                offerDto.ConfirmedByReceivingUser = waterTransfer.ConfirmedByReceivingUser;
                offerDto.DateConfirmedByReceivingUser = waterTransfer.DateConfirmedByReceivingUser;
                offerDto.WaterTransferID = waterTransfer.WaterTransferID;
            }

            return offerDto;
        }
    }
}