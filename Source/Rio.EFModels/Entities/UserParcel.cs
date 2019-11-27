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

            var previousParcelOwnershipRecord = dbContext.UserParcel.SingleOrDefault(x =>
                x.ParcelID == parcelIDToChange && x.EffectiveYear == effectiveYear);

            if (previousParcelOwnershipRecord != null)
            {
                previousParcelOwnershipRecord.EffectiveYear = null;
            }

            var newParcelOwnershipRecord = new UserParcel()
            {
                ParcelID = parcelIDToChange,
                UserID = parcelChangeOwnerDto.UserID,
                OwnerName = parcelChangeOwnerDto.OwnerName,
                EffectiveYear = effectiveYear,
                SaleDate = parcelChangeOwnerDto.SaleDate,
                Note = parcelChangeOwnerDto.Note
            };

            dbContext.UserParcel.Add(newParcelOwnershipRecord);

            dbContext.SaveChanges();
        }
    }
}