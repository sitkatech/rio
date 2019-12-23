using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public static class vParcelOwnershipExtensionMethods
    {
        public static ParcelDto AsParcelDto(this vParcelOwnership vParcelOwnership)
        {
            AccountSimpleDto landOwner;

            if (vParcelOwnership.Account != null)
            {
                landOwner = vParcelOwnership.Account.AsSimpleDto();
            }
            else
            {
                landOwner = new AccountSimpleDto()
                {
                    AccountName = vParcelOwnership.OwnerName
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
            var account = vParcelOwnership.Account;
            var parcelOwnershipDto = new ParcelOwnershipDto
            {
                OwnerName = account != null ? $"{account.AccountName}" : vParcelOwnership.OwnerName,
                OwnerAccountID = account?.AccountID,
                EffectiveYear = vParcelOwnership.EffectiveYear ?? vParcelOwnership.SaleDate?.Year,
                Note = vParcelOwnership.Note,
                SaleDate = vParcelOwnership.SaleDate?.ToShortDateString() ?? ""

            };

            return parcelOwnershipDto;
        }
    }
}