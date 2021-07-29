using Rio.Models.DataTransferObjects.Offer;
using System.Linq;

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
                CreateAccount = offer.CreateAccount.AsDto(),
                OfferStatus = offer.OfferStatus.AsDto(),
                Trade = offer.Trade.AsDto(),
            };
            var waterTransfer = offer.WaterTransfers.SingleOrDefault();
            if (waterTransfer != null)
            {
                offerDto.WaterTransferID = waterTransfer.WaterTransferID;
            }

            return offerDto;
        }
    }
}