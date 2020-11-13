using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rio.API.Util;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public partial class User
    {
        public static UserDto CreateNewUser(RioDbContext dbContext, UserUpsertDto userToCreate, string loginName, Guid userGuid)
        {
            if (!userToCreate.RoleID.HasValue)
            {
                return null;
            }

            var user = new User
            {
                UserGuid = userGuid,
                LoginName = loginName,
                Email = userToCreate.Email,
                FirstName = userToCreate.FirstName,
                LastName = userToCreate.LastName,
                RoleID = userToCreate.RoleID.Value,
                CreateDate = DateTime.UtcNow,
            };

            dbContext.User.Add(user);
            dbContext.SaveChanges();
            dbContext.Entry(user).Reload();

            return GetByUserID(dbContext, user.UserID);
        }

        public static IEnumerable<UserDetailedDto> List(RioDbContext dbContext)
        {
            var accountUsers = dbContext.AccountUser
                .Include(x => x.Account)
                .ToList()
                .GroupBy(x => x.UserID)
                .Select(x => new {UserID = x.Key, Accounts = x.Select(y => y.Account.AsSimpleDto()).ToList()});

            // right now we are assuming a parcel can only be associated to one user
            var users = dbContext.vUserDetailed
                .ToList()
                .GroupJoin(accountUsers,
                    x => x.UserID,
                    y => y.UserID,
                    (x,y) => new {vUserDetailed = x, Accounts = y.DefaultIfEmpty()})
                .OrderBy(x => x.vUserDetailed.LastName).ThenBy(x => x.vUserDetailed.FirstName).ToList()
                .Select(user =>
                {
                    var userDetailedDto = new UserDetailedDto()
                    {
                        UserID = user.vUserDetailed.UserID,
                        UserGuid = user.vUserDetailed.UserGuid,
                        FirstName = user.vUserDetailed.FirstName,
                        LastName = user.vUserDetailed.LastName,
                        Email = user.vUserDetailed.Email,
                        LoginName = user.vUserDetailed.LoginName,
                        RoleID = user.vUserDetailed.RoleID,
                        RoleDisplayName = user.vUserDetailed.RoleDisplayName,
                        Phone = user.vUserDetailed.Phone,
                        HasActiveTrades = user.vUserDetailed.HasActiveTrades,
                        AcreFeetOfWaterPurchased = user.vUserDetailed.AcreFeetOfWaterPurchased,
                        AcreFeetOfWaterSold = user.vUserDetailed.AcreFeetOfWaterSold,
                        ReceiveSupportEmails = user.vUserDetailed.ReceiveSupportEmails,
                        AssociatedAccounts = user.Accounts.Select(x => x?.Accounts).SingleOrDefault()
                    };
                    return userDetailedDto;
                }).ToList();
            return users;
        }

        public static IEnumerable<UserDto> ListByRole(RioDbContext dbContext, RoleEnum roleEnum)
        {
            var users = GetUserImpl(dbContext)
                .Where(x => x.RoleID == (int) roleEnum)
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return users;
        }

        public static IEnumerable<UserDto> ListByRole(RioDbContext dbContext, List<int> roles)
        {
            var users = GetUserImpl(dbContext)
                .Where(x => roles.Contains(x.RoleID))
                .Select(x => x.AsDto())
                .AsEnumerable();

            return users;
        }

        public static IEnumerable<string> GetEmailAddressesForAdminsThatReceiveSupportEmails(RioDbContext dbContext)
        {
            var users = GetUserImpl(dbContext)
                .Where(x => x.RoleID == (int) RoleEnum.Admin && x.ReceiveSupportEmails)
                .Select(x => x.Email)
                .AsEnumerable();

            return users;
        }

        public static UserDto GetByUserID(RioDbContext dbContext, int userID)
        {
            var user = GetUserImpl(dbContext).SingleOrDefault(x => x.UserID == userID);
            return user?.AsDto();
        }

        public static List<UserDto> GetByUserID(RioDbContext dbContext, List<int> userIDs)
        {
            return GetUserImpl(dbContext).Where(x => userIDs.Contains(x.UserID)).Select(x=>x.AsDto()).ToList();
            
        }

        public static UserDto GetByUserGuid(RioDbContext dbContext, Guid userGuid)
        {
            var user = GetUserImpl(dbContext)
                .SingleOrDefault(x => x.UserGuid == userGuid);

            return user?.AsDto();
        }

        private static IQueryable<User> GetUserImpl(RioDbContext dbContext)
        {
            return dbContext.User
                .Include(x => x.Role)
                .AsNoTracking();
        }

        public static UserDto GetByEmail(RioDbContext dbContext, string email)
        {
            var user = GetUserImpl(dbContext).SingleOrDefault(x => x.Email == email);
            return user?.AsDto();
        }

        public static UserDto UpdateUserEntity(RioDbContext dbContext, int userID, UserUpsertDto userEditDto)
        {
            if (!userEditDto.RoleID.HasValue)
            {
                return null;
            }

            var user = dbContext.User
                .Include(x => x.Role)
                .Single(x => x.UserID == userID);

            if (user.RoleID != (int)RoleEnum.Admin && userEditDto.RoleID == (int)RoleEnum.Admin)
            {
                dbContext.AccountUser.RemoveRange(dbContext.AccountUser.Where(x => x.UserID == user.UserID));
            }

            user.RoleID = userEditDto.RoleID.Value;

            user.ReceiveSupportEmails = userEditDto.RoleID.Value == 1 && userEditDto.ReceiveSupportEmails;
            user.UpdateDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(user).Reload();
            return GetByUserID(dbContext, userID);
        }

        public static UserDto SetDisclaimerAcknowledgedDate(RioDbContext dbContext, int userID)
        {
            var user = dbContext.User.Single(x => x.UserID == userID);

            user.UpdateDate = DateTime.UtcNow;
            user.DisclaimerAcknowledgedDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(user).Reload();

            return GetByUserID(dbContext, userID);
        }

        public static UserDto UpdateUserGuid(RioDbContext dbContext, int userID, Guid userGuid)
        {
            var user = dbContext.User
                .Single(x => x.UserID == userID);

            user.UserGuid = userGuid;
            user.UpdateDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(user).Reload();
            return GetByUserID(dbContext, userID);
        }

        public static List<ErrorMessage> ValidateUpdate(RioDbContext dbContext, UserUpsertDto userEditDto, int userID)
        {
            var result = new List<ErrorMessage>();
            if (!userEditDto.RoleID.HasValue)
            {
                result.Add(new ErrorMessage() { Type = "Role ID", Message = "Role ID is required." });
            }

            return result;
        }

        public static bool ValidateAllExist(RioDbContext dbContext, List<int> userIDs)
        {
            return dbContext.User.Count(x => userIDs.Contains(x.UserID)) == userIDs.Distinct().Count();
        }

        public static UserDto SetAssociatedAccounts(RioDbContext dbContext, int userID, List<int> accountIDs,
            out List<int> addedAccountIDs)
        {
            var newAccountUsers = accountIDs.Select(accountID => new AccountUser() { UserID = userID, AccountID = accountID }).ToList();

            var existingAccountUsers = dbContext.User.Include(x => x.AccountUser)
                .Single(x => x.UserID == userID).AccountUser;

            addedAccountIDs = accountIDs.Where(x => !existingAccountUsers.Select(y => y.AccountID).Contains(x)).ToList();

            var allInDatabase = dbContext.AccountUser;

            existingAccountUsers.Merge(newAccountUsers, allInDatabase, (x, y) => x.UserID == y.UserID && x.AccountID == y.AccountID);

            dbContext.SaveChanges();

            return GetByUserID(dbContext, userID);
        }

        public static bool CheckIfUsersAreAdministrators(RioDbContext dbContext, List<int> userIDs)
        {
            return dbContext.User.Any(x => userIDs.Contains(x.UserID) && x.RoleID == (int) RoleEnum.Admin);
        }
    }
}