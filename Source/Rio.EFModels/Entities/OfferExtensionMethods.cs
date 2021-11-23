using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class OfferExtensionMethods
    {
        static partial void DoCustomMappings(Offer offer, OfferDto offerDto)
        {
            var waterTransfer = offer.WaterTransfers.SingleOrDefault();
            if (waterTransfer != null)
            {
                offerDto.WaterTransferID = waterTransfer.WaterTransferID;
            }
        }
    }
}