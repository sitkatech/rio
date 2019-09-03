using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public static class WaterTransferExtensionMethods
    {
        public static WaterTransferDto AsDto(this WaterTransfer waterTransfer)
        {
            return new WaterTransferDto()
            {
                WaterTransferID = waterTransfer.WaterTransferID,
                OfferID = waterTransfer.OfferID,
                TransferDate = waterTransfer.TransferDate,
                TransferYear = waterTransfer.TransferDate.Year,
                AcreFeetTransferred = waterTransfer.AcreFeetTransferred,
                UnitPrice = waterTransfer.Offer?.Price,
                TransferringUser = waterTransfer.TransferringUser.AsSimpleDto(),
                ReceivingUser = waterTransfer.ReceivingUser.AsSimpleDto(),
                ConfirmedByTransferringUser = waterTransfer.ConfirmedByTransferringUser,
                ConfirmedByReceivingUser = waterTransfer.ConfirmedByReceivingUser,
                Notes = waterTransfer.Notes
            };
        }
    }
}