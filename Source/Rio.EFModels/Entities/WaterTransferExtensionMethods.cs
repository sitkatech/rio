using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferExtensionMethods
    {
        public static WaterTransferDetailedDto AsDetailedDto(this WaterTransfer waterTransfer)
        {
            var sellerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Selling);
            var buyerRegistration = waterTransfer.GetWaterTransferRegistrationByWaterTransferType(WaterTransferTypeEnum.Buying);

            return new WaterTransferDetailedDto()
            {
                WaterTransferID = waterTransfer.WaterTransferID,
                OfferID = waterTransfer.OfferID,
                TransferDate = waterTransfer.TransferDate,
                TransferYear = waterTransfer.TransferDate.Year,
                AcreFeetTransferred = waterTransfer.AcreFeetTransferred,
                UnitPrice = waterTransfer.Offer?.Price,
                SellerRegistration = sellerRegistration.AsDto(),
                BuyerRegistration = buyerRegistration.AsDto(),
                Notes = waterTransfer.Notes,
                // ReSharper disable once PossibleNullReferenceException
                TradeNumber = waterTransfer.Offer.Trade.TradeNumber
            };
        }
    }
}