using System.Linq;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static class ParcelExtensionMethods
    {
        public static ParcelDto AsDto(this Parcel parcel)
        {
            return new ParcelDto()
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                LandOwner = parcel.UserParcel.OrderBy(x => x.UserID).FirstOrDefault()?.User.AsSimpleDto()
            };
        }
    }
}