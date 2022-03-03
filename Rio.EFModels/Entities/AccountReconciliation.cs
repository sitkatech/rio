using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class AccountReconciliation
    {
        public static List<AccountReconciliationCustomDto> List(RioDbContext dbContext)
        {
            return dbContext.AccountReconciliations
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => new AccountReconciliationCustomDto()
                {
                    Parcel = x.First().Parcel.AsSimpleDto(),
                    LastKnownOwner = vParcelOwnership.GetLastOwnerOfParcelByParcelID(dbContext, x.Key),
                    AccountsClaimingOwnership = x.Select(y => y.Account.AsSimpleDto()).ToList()
                }).ToList();
        }

        public static List<ParcelSimpleDto> ListParcelsByAccountID(RioDbContext dbContext, int accountId)
        {
            return dbContext.AccountReconciliations
                .Include(x => x.Parcel)
                .Where(x => x.AccountID == accountId)
                .Select(x => x.Parcel.AsSimpleDto())
                .ToList();
        }

        public static void DeleteByParcelID(RioDbContext dbContext, int parcelId)
        {
            var toRemove = dbContext.AccountReconciliations.Where(x => x.ParcelID == parcelId);

            dbContext.AccountReconciliations.RemoveRange(toRemove);

            dbContext.SaveChanges();
        }
    }
}