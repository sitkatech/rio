using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using Rio.API.Util;
using Rio.Models.DataTransferObjects.ReconciliationAllocation;

namespace Rio.EFModels.Entities
{
    public partial class Account
    {
        public static List<AccountSimpleDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            return dbContext.User.Include(x => x.AccountUser).ThenInclude(x => x.Account)
                .Single(x => x.UserID == userID).AccountUser.Select(x => x.Account.AsSimpleDto()).ToList();
        }

        public static List<AccountDto> List(RioDbContext dbContext)
        {
            return dbContext.Account.Include(x => x.AccountStatus).Include(x=>x.AccountParcel).Include(x => x.AccountUser).ThenInclude(x => x.User).Select(x => x.AsDto())
                .ToList();
        }

        public static AccountDto GetByAccountID(RioDbContext dbContext, int accountID)
        {
            return dbContext.Account.Include(x => x.AccountStatus).Include(x => x.AccountUser).ThenInclude(x => x.User)
                .Single(x => x.AccountID == accountID).AsDto();
        }

        public static List<AccountDto> GetByAccountID(RioDbContext dbContext, List<int> accountIDs)
        {
            return dbContext.Account.Include(x => x.AccountStatus).Include(x => x.AccountUser).ThenInclude(x => x.User)
                .Where(x => accountIDs.Contains(x.AccountID)).Select(x=>x.AsDto()).ToList();
        }

        public static AccountDto UpdateAccountEntity(RioDbContext dbContext, int accountID, AccountUpdateDto accountUpdateDto)
        {
            var account = dbContext.Account
                .Include(x => x.AccountStatus)
                .Single(x => x.AccountID == accountID);

            account.AccountStatusID = accountUpdateDto.AccountStatusID;
            account.Notes = accountUpdateDto.Notes;
            account.AccountName = accountUpdateDto.AccountName;
            account.UpdateDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(account).Reload();
            return GetByAccountID(dbContext, accountID);
        }

        public static AccountDto CreateAccountEntity(RioDbContext dbContext, AccountUpdateDto accountUpdateDto)
        {
            var account = new Account()
            {
                AccountStatusID = accountUpdateDto.AccountStatusID,
                Notes = accountUpdateDto.Notes,
                AccountName = accountUpdateDto.AccountName,
                UpdateDate = DateTime.UtcNow
            };

            dbContext.Account.Add(account);
            dbContext.SaveChanges();
            dbContext.Entry(account).Reload();

            return GetByAccountID(dbContext, account.AccountID);
        }

        public static AccountDto SetAssociatedUsers(RioDbContext dbContext, AccountDto accountDto, List<int> userIDs, out List<int> addedUserIDs)
        {
            var newAccountUsers = userIDs.Select(userID => new AccountUser(){AccountID = accountDto.AccountID, UserID = userID}).ToList();

            var existingAccountUsers = dbContext.Account.Include(x => x.AccountUser)
                .Single(x => x.AccountID == accountDto.AccountID).AccountUser;

            addedUserIDs = userIDs.Where(x => !existingAccountUsers.Select(y => y.UserID).Contains(x)).ToList();

            var allInDatabase = dbContext.AccountUser;

            existingAccountUsers.Merge(newAccountUsers, allInDatabase, (x,y)=>x.AccountID == y.AccountID && x.UserID == y.UserID);

            dbContext.SaveChanges();

            return GetByAccountID(dbContext, accountDto.AccountID);
        }

        public static bool ValidateAllExist(RioDbContext dbContext, List<int> accountIDs)
        {
            return dbContext.Account.Count(x => accountIDs.Contains(x.AccountID)) == accountIDs.Distinct().Count();
        }

        public static void SetReconciliationAllocation(RioDbContext dbContext, List<ReconciliationAllocationCSV> records, int waterYear)
        {
            // delete existing parcel allocations
            var existingParcelAllocations = dbContext.ParcelAllocation.Where(x =>
                x.WaterYear == waterYear && x.ParcelAllocationTypeID == (int) ParcelAllocationTypeEnum.Reconciliation);
            if (existingParcelAllocations.Any())
            {
                dbContext.ParcelAllocation.RemoveRange(existingParcelAllocations);
                dbContext.SaveChanges();
            }

            var accountReconciliationVolumes = Parcel.vParcelOwnershipsByYear(dbContext,waterYear).ToList().GroupBy(x => x.Account.AccountNumber)
                .Where(x => records.Select(y => y.AccountNumber).Contains(x.Key)).Join(records,
                    account => account.Key, record => record.AccountNumber,
                    (x, y) => new {Parcels = x.Select(y=>y.Parcel).ToList(), ReconciliationVolume = y.ReconciliationVolume});

             //dbContext.Account.Include(x => x.AccountParcel).ThenInclude(x => x.Parcel)
             //   .Where(x => records.Select(y => y.AccountNumber).Contains(x.AccountNumber)).ToList().Join(records,
             //       account => account.AccountNumber, record => record.AccountNumber,
             //       (x, y) => new {Account = x, ReconciliationVolume = y.ReconciliationVolume});

            var parcelAllocations = new List<ParcelAllocation>();

            foreach (var record in accountReconciliationVolumes)
            {
                var parcels = record.Parcels;
                var sum = parcels.Sum(x=>x.ParcelAreaInAcres);
                parcelAllocations.AddRange(parcels.Select(x => new ParcelAllocation()
                {
                    ParcelID = x.ParcelID,
                    AcreFeetAllocated =
                        (decimal) (record.ReconciliationVolume * (x.ParcelAreaInAcres / sum)),
                    WaterYear = waterYear,
                    ParcelAllocationTypeID = (int) ParcelAllocationTypeEnum.Reconciliation
                }));
            }

            dbContext.ParcelAllocation.AddRange(parcelAllocations);
            dbContext.SaveChanges();
        }
    }
}
