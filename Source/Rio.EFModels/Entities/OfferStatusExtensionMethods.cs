using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public static class OfferStatusExtensionMethods
    {
        public static OfferStatusDto AsDto(this OfferStatus offerStatus)
        {
            return new OfferStatusDto()
            {
                OfferStatusID = offerStatus.OfferStatusID,
                OfferStatusName = offerStatus.OfferStatusName,
                OfferStatusDisplayName = offerStatus.OfferStatusDisplayName
            };
        }
    }
}