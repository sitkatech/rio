using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Account;

namespace Rio.EFModels.Entities
{
    public partial class vParcelOwnership
    {
        [ForeignKey(nameof(ParcelID))]
        public virtual Parcel Parcel { get; set; }

        [ForeignKey(nameof(AccountID))]
        public virtual Account Account { get; set; }
        
        [ForeignKey(nameof(WaterYearID))]
        public virtual WaterYear WaterYear { get; set; }

        public static AccountSimpleDto GetLastOwnerOfParcelByParcelID(RioDbContext dbContext, int parcelID)
        {
            var allOwners = dbContext.vParcelOwnerships
                .Include(x => x.WaterYear)
                .Include(x => x.Account)
                .Where(x => x.ParcelID == parcelID && x.AccountID != null);

            return !allOwners.Any() ? null :
                allOwners.OrderByDescending(x => x.WaterYear.Year)
                .FirstOrDefault()
                .Account?.AsSimpleDto();
        }
    }
}