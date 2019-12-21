using System.Linq;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class UserParcel
    {


        public static void ChangeParcelOwner(RioDbContext dbContext, int parcelID, ParcelChangeOwnerDto parcelChangeOwnerDto)
        {
            var parcelIDToChange = parcelID;

            var effectiveYear = parcelChangeOwnerDto.EffectiveYear ?? parcelChangeOwnerDto.SaleDate.Year;

            var previousParcelOwnershipRecord = dbContext.AccountParcel.SingleOrDefault(x =>
                x.ParcelID == parcelIDToChange && x.EffectiveYear == effectiveYear);

            if (previousParcelOwnershipRecord != null)
            {
                previousParcelOwnershipRecord.EffectiveYear = null;
            }

            var newParcelOwnershipRecord = new AccountParcel()
            {
                ParcelID = parcelIDToChange,
                AccountID = parcelChangeOwnerDto.AccountID,
                OwnerName = parcelChangeOwnerDto.OwnerName,
                EffectiveYear = effectiveYear,
                SaleDate = parcelChangeOwnerDto.SaleDate,
                Note = parcelChangeOwnerDto.Note
            };

            dbContext.AccountParcel.Add(newParcelOwnershipRecord);

            dbContext.SaveChanges();
        }
    }
}