using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public static class WaterTransferParcelExtensionMethods
    {
        public static WaterTransferParcelDto AsDto(this WaterTransferParcel waterTransferParcel)
        {
            return new WaterTransferParcelDto()
            {
                ParcelID = waterTransferParcel.ParcelID,
                ParcelNumber = waterTransferParcel.Parcel.ParcelNumber,
                AcreFeetTransferred = waterTransferParcel.AcreFeetTransferred,
                WaterTransferTypeID = waterTransferParcel.WaterTransferTypeID
            };
        }
    }
}