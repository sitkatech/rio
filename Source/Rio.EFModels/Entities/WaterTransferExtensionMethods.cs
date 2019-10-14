using System.Linq;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public static class WaterTransferExtensionMethods
    {
        public static WaterTransferDto AsDto(this WaterTransfer waterTransfer)
        {
            var sellerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Selling);
            var buyerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Buying);

            return new WaterTransferDto()
            {
                WaterTransferID = waterTransfer.WaterTransferID,
                OfferID = waterTransfer.OfferID,
                TransferDate = waterTransfer.TransferDate,
                TransferYear = waterTransfer.TransferDate.Year,
                AcreFeetTransferred = waterTransfer.AcreFeetTransferred,
                UnitPrice = waterTransfer.Offer?.Price,
                SellerRegistration = sellerRegistration.AsSimpleDto(),
                BuyerRegistration = buyerRegistration.AsSimpleDto(),
                Notes = waterTransfer.Notes,
                // ReSharper disable once PossibleNullReferenceException
                TradeNumber = waterTransfer.Offer.Trade.TradeNumber
            };
        }
    }
}