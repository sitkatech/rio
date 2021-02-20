using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class AccountReconciliation
    {
        public static List<AccountReconciliationDto> List(RioDbContext dbContext)
        {
            return dbContext.AccountReconciliation
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => new AccountReconciliationDto()
                {
                    Parcel = x.First().Parcel.AsSimpleDto(),
                    LastKnownOwner = vParcelOwnership.GetLastOwnerOfParcelByParcelID(dbContext, x.Key),
                    AccountsClaimingOwnership = x.Select(y => y.Account.AsSimpleDto()).ToList()
                }).ToList();
        }

        public static List<ParcelSimpleDto> ListParcelsByAccountID(RioDbContext dbContext, int accountId)
        {
            return dbContext.AccountReconciliation
                .Include(x => x.Parcel)
                .Where(x => x.AccountID == accountId)
                .Select(x => x.Parcel.AsSimpleDto())
                .ToList();
        }

        public static void DeleteByParcelID(RioDbContext dbContext, int parcelId)
        {
            var toRemove = dbContext.AccountReconciliation.Where(x => x.ParcelID == parcelId);

            dbContext.AccountReconciliation.RemoveRange(toRemove);

            dbContext.SaveChanges();
        }
    }
}