using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public static class vParcelOwnershipExtensionMethods
    {
        public static ParcelDto AsParcelDto(this vParcelOwnership vParcelOwnership)
        {
            UserSimpleDto landOwner;

            if (vParcelOwnership.User != null)
            {
                landOwner = vParcelOwnership.User.AsSimpleDto();
            }
            else
            {
                landOwner = new UserSimpleDto()
                {
                    FirstName = vParcelOwnership.OwnerName
                };
            }

            var parcelDto = new ParcelDto
            {
                ParcelID = vParcelOwnership.ParcelID,
                ParcelNumber = vParcelOwnership.Parcel.ParcelNumber,
                ParcelAreaInAcres = vParcelOwnership.Parcel.ParcelAreaInAcres,
                LandOwner = landOwner

            };

            return parcelDto;
        }
        public static ParcelOwnershipDto AsParcelOwnershipDto(this vParcelOwnership vParcelOwnership)
        {
            var user = vParcelOwnership.User;
            var parcelOwnershipDto = new ParcelOwnershipDto
            {
                OwnerName = user != null ? $"{user.FirstName} {user.LastName}" : vParcelOwnership.OwnerName,
                OwnerUserID = user?.UserID,
                EffectiveYear = vParcelOwnership.EffectiveYear ?? vParcelOwnership.SaleDate?.Year,
                Note = vParcelOwnership.Note,
                SaleDate = vParcelOwnership.SaleDate?.ToShortDateString() ?? ""

            };

            return parcelOwnershipDto;
        }
    }
}