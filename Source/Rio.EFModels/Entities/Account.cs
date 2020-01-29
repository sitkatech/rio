using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using Rio.API.Util;

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

        public static AccountDto SetAssociatedUsers(RioDbContext dbContext, AccountDto accountDto, List<int> userIDs)
        {
            var newAccountUsers = userIDs.Select(userID => new AccountUser(){AccountID = accountDto.AccountID, UserID = userID}).ToList();

            var existingAccountUsers = dbContext.Account.Include(x => x.AccountUser)
                .Single(x => x.AccountID == accountDto.AccountID).AccountUser;

            var allInDatabase = dbContext.AccountUser;

            existingAccountUsers.Merge(newAccountUsers, allInDatabase, (x,y)=>x.AccountID == y.AccountID && x.UserID == y.UserID);

            dbContext.SaveChanges();

            return GetByAccountID(dbContext, accountDto.AccountID);
        }

        public static bool ValidateAllExist(RioDbContext dbContext, List<int> accountIDs)
        {
            return dbContext.Account.Count(x => accountIDs.Contains(x.AccountID)) == accountIDs.Distinct().Count();
        }
    }
}
