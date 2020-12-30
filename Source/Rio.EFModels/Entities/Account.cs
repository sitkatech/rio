using Microsoft.EntityFrameworkCore;
using Rio.API.Util;
using Rio.Models.DataTransferObjects.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Rio.EFModels.Entities
{
    public partial class Account
    {
        public static List<AccountSimpleDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            return dbContext.User.Include(x => x.AccountUser).ThenInclude(x => x.Account)
                .Single(x => x.UserID == userID).AccountUser
                .OrderBy(x => x.Account.AccountName)
                .Select(x => x.Account.AsSimpleDto()).ToList();
        }

        public static List<AccountIncludeParcelsDto> ListByUserIDIncludeParcels(RioDbContext dbContext, int userID)
        {
            return dbContext.User
                .Include(x => x.AccountUser)
                .ThenInclude(x => x.Account)
                .ThenInclude(x => x.AccountStatus)
                .Include(x => x.AccountUser)
                .ThenInclude(x => x.Account)
                .ThenInclude(x => x.AccountUser)
                .ThenInclude(x => x.User)
                .Include(x => x.AccountUser)
                .ThenInclude(x => x.Account)
                .ThenInclude(x => x.AccountParcel)
                .ThenInclude(x => x.Parcel)
                .Single(x => x.UserID == userID).AccountUser
                .OrderBy(x => x.Account.AccountName)
                .Select(x => x.Account.AsAccountWithParcelsDto()).ToList();
        }

        public static List<AccountIncludeParcelsDto> ListIncludeParcels(RioDbContext dbContext)
        {
            return dbContext.Account
                .Include(x => x.AccountStatus)
                .Include(x => x.AccountParcel)
                .ThenInclude(x => x.Parcel)
                .Include(x => x.AccountUser)
                .ThenInclude(x => x.User)
                .OrderBy(x => x.AccountName)
                .Select(x => x.AsAccountWithParcelsDto())
                .ToList();
        }

        public static List<AccountDto> List(RioDbContext dbContext)
        {
            return dbContext.Account
                .Include(x => x.AccountStatus)
                .Include(x=>x.AccountParcel)
                .Include(x => x.AccountUser)
                .ThenInclude(x => x.User)
                .OrderBy(x => x.AccountName)
                .Select(x => x.AsDto())
                .ToList();
        }

        public static AccountDto GetByAccountID(RioDbContext dbContext, int accountID)
        {
            return dbContext.Account.Include(x => x.AccountStatus).Include(x => x.AccountUser).ThenInclude(x => x.User)
                .SingleOrDefault(x => x.AccountID == accountID)?.AsDto();
        }

        public static AccountDto GetByAccountVerificationKey(RioDbContext dbContext, string accountVerificationKey)
        {
            if (String.IsNullOrEmpty(accountVerificationKey))
            {
                return null;
            }

            return dbContext.Account.Include(x => x.AccountStatus).Include(x => x.AccountUser).ThenInclude(x => x.User)
                .SingleOrDefault(x => x.AccountVerificationKey == accountVerificationKey)?.AsDto();
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

        public static AccountDto CreateAccountEntity(RioDbContext dbContext, AccountUpdateDto accountUpdateDto,
            string rioConfigurationVerificationKeyChars)
        {
            var account = new Account()
            {
                AccountStatusID = accountUpdateDto.AccountStatusID,
                Notes = accountUpdateDto.Notes,
                AccountName = accountUpdateDto.AccountName,
                UpdateDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
                AccountVerificationKey = GenerateAndVerifyAccountVerificationKey(rioConfigurationVerificationKeyChars, GetCurrentAccountVerificationKeys(dbContext))
            };

            dbContext.Account.Add(account);
            dbContext.SaveChanges();
            dbContext.Entry(account).Reload();

            return GetByAccountID(dbContext, account.AccountID);
        }

        public static List<string> GetCurrentAccountVerificationKeys(RioDbContext dbContext)
        {
            return dbContext.Account.Select(x => x.AccountVerificationKey).ToList();
        }

        private static string GenerateAndVerifyAccountVerificationKey(string rioConfigurationVerificationKeyChars,
            List<string> currentAccountVerificationKeys)
        {
            var accountVerificationKey = GenerateAccountVerificationKey(rioConfigurationVerificationKeyChars);
            while (currentAccountVerificationKeys.Contains(accountVerificationKey))
            {
                accountVerificationKey = GenerateAccountVerificationKey(rioConfigurationVerificationKeyChars);
            }

            return accountVerificationKey;
        }

        private static string GenerateAccountVerificationKey(string rioConfigurationVerificationKeyChars)
        {

            var applicableVerificationKeyChars = rioConfigurationVerificationKeyChars.Split(',');
            Random random = new Random();

            return 
                new string(Enumerable.Repeat(applicableVerificationKeyChars[0], 3).Select(x => x[random.Next(x.Length)]).ToArray()) + 
                new string(Enumerable.Repeat(applicableVerificationKeyChars[1], 3).Select(x => x[random.Next(x.Length)]).ToArray());
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

        public static void BulkInactivate(RioDbContext dbContext, List<Account> accountsToInactivate, bool saveChanges)
        {
            accountsToInactivate.ForEach(x =>
            {
                x.UpdateDate = DateTime.UtcNow;
                x.InactivateDate = DateTime.UtcNow;
                x.AccountStatusID = (int) AccountStatusEnum.Inactive;
                x.AccountVerificationKey = null;
            });

            if (saveChanges)
            {
                dbContext.SaveChanges();
            }
        }

        public static void BulkCreateWithListOfNames(RioDbContext dbContext, string rioConfigurationVerificationKeyChars, List<string> accountNamesToCreate, bool saveChanges)
        {
            var listOfAccountsToCreate = new List<Account>();
            var currentAccountVerificationKeys = GetCurrentAccountVerificationKeys(dbContext);

            accountNamesToCreate.ForEach(x =>
            {
                var accountVerificationKey =
                    GenerateAndVerifyAccountVerificationKey(rioConfigurationVerificationKeyChars,
                        currentAccountVerificationKeys);
                currentAccountVerificationKeys.Add(accountVerificationKey);

                listOfAccountsToCreate.Add(new Account()
                {
                    AccountStatusID = (int)AccountStatusEnum.Active,
                    AccountName = x,
                    UpdateDate = DateTime.UtcNow,
                    CreateDate = DateTime.UtcNow,
                    AccountVerificationKey = accountVerificationKey
                });
            });

            dbContext.Account.AddRange(listOfAccountsToCreate);

            if (saveChanges)
            {
                dbContext.SaveChanges();
            }
        }
    }
}
