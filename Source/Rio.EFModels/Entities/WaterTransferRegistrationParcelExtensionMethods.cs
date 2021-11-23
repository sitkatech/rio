using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferRegistrationParcelExtensionMethods
    {
        public static WaterTransferRegistrationParcelUpsertDto AsUpsertDto(this WaterTransferRegistrationParcel waterTransferRegistrationParcel)
        {
            return new WaterTransferRegistrationParcelUpsertDto
            {
                ParcelID = waterTransferRegistrationParcel.ParcelID,
                ParcelNumber = waterTransferRegistrationParcel.Parcel.ParcelNumber,
                AcreFeetTransferred = waterTransferRegistrationParcel.AcreFeetTransferred
            };
        }
    }
}