using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static class ParcelStatusExtensionMethods
    {
        public static ParcelStatusDto AsDto(this ParcelStatus parcelStatus)
        {
            return new ParcelStatusDto()
            {
                ParcelStatusID = parcelStatus?.ParcelStatusID,
                ParcelStatusDisplayName = parcelStatus?.ParcelStatusDisplayName
            };
        }
    }
}