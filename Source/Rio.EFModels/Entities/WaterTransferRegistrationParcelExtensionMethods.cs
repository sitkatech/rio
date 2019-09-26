using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public static class WaterTransferRegistrationParcelExtensionMethods
    {
        public static WaterTransferRegistrationParcelDto AsDto(this WaterTransferRegistrationParcel waterTransferRegistrationParcel)
        {
            return new WaterTransferRegistrationParcelDto
            {
                ParcelID = waterTransferRegistrationParcel.ParcelID,
                ParcelNumber = waterTransferRegistrationParcel.Parcel.ParcelNumber,
                AcreFeetTransferred = waterTransferRegistrationParcel.AcreFeetTransferred
            };
        }
    }
}