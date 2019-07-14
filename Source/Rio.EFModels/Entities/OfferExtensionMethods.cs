using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public static class OfferExtensionMethods
    {
        public static OfferDto AsDto(this Offer offer)
        {
            return new OfferDto()
            {
                OfferID = offer.OfferID,
                OfferDate = offer.OfferDate,
                OfferNotes = offer.OfferNotes,
                Quantity = offer.Quantity,
                Price = offer.Price,
                CreateUser = offer.CreateUser.AsSimpleDto(),
                OfferStatus = offer.OfferStatus.AsDto(),
                TradeID = offer.TradeID
            };
        }
    }
}