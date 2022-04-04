using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.API.Util;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public partial class User
    {
        public static UserDto CreateUnassignedUser(RioDbContext dbContext, UserCreateDto userCreateDto)
        {
            var userUpsertDto = new UserUpsertDto()
            {
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                OrganizationName = userCreateDto.OrganizationName,
                Email = userCreateDto.Email,
                PhoneNumber = userCreateDto.PhoneNumber,
                RoleID = (int)RoleEnum.Unassigned,  // don't allow non-admin user to set their role to something other than Unassigned
                ReceiveSupportEmails = false  // don't allow non-admin users to hijack support emails
            };
            return CreateNewUser(dbContext, userUpsertDto, userCreateDto.LoginName, userCreateDto.UserGuid);
        }

        public static List<ErrorMessage> ValidateCreateUnassignedUser(RioDbContext dbContext, UserCreateDto userCreateDto)
        {
            var result = new List<ErrorMessage>();

            var userByGuidDto = GetByUserGuid(dbContext, userCreateDto.UserGuid);  // A duplicate Guid not only leads to 500s, it allows someone to hijack another user's account
            if (userByGuidDto != null)
            {
                result.Add(new ErrorMessage() { Type = "User Creation", Message = "Invalid user information." });  // purposely vague; we don't want a naughty person realizing they figured out someone else's Guid
            }

            var userByEmailDto = GetByEmail(dbContext, userCreateDto.Email);  // A duplicate email leads to 500s, so need to prevent duplicates
            if (userByEmailDto != null)
            {
                result.Add(new ErrorMessage() { Type = "User Creation", Message = "There is already a user account with this email address." });
            }

            return result;
        }
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

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            dbContext.Entry(user).Reload();

            return GetByUserID(dbContext, user.UserID);
        }

        public static IEnumerable<UserDetailedDto> List(RioDbContext dbContext)
        {
            var accountUsers = dbContext.AccountUsers
                .Include(x => x.Account)
                .ToList()
                .GroupBy(x => x.UserID)
                .Select(x => new {UserID = x.Key, Accounts = x.Select(y => y.Account.AsSimpleDto()).ToList()});

            // right now we are assuming a parcel can only be associated to one user
            var users = dbContext.vUserDetaileds
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
            return dbContext.Users
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

            var user = dbContext.Users.Single(x => x.UserID == userID);

            if (user.RoleID != (int)RoleEnum.Admin && userEditDto.RoleID == (int)RoleEnum.Admin)
            {
                dbContext.AccountUsers.RemoveRange(dbContext.AccountUsers.Where(x => x.UserID == user.UserID));
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
            var user = dbContext.Users.Single(x => x.UserID == userID);

            user.UpdateDate = DateTime.UtcNow;
            user.DisclaimerAcknowledgedDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(user).Reload();

            return GetByUserID(dbContext, userID);
        }

        public static UserDto UpdateUserGuid(RioDbContext dbContext, int userID, Guid userGuid)
        {
            var user = dbContext.Users
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
            return dbContext.Users.Count(x => userIDs.Contains(x.UserID)) == userIDs.Distinct().Count();
        }

        public static List<int> SetAssociatedAccounts(RioDbContext dbContext, int userID, List<int> accountIDs)
        {
            var newAccountUsers = accountIDs.Select(accountID => new AccountUser() { UserID = userID, AccountID = accountID }).ToList();

            var existingAccountUsers = dbContext.Users.Include(x => x.AccountUsers)
                .Single(x => x.UserID == userID).AccountUsers;

            var addedAccountIDs = accountIDs.Where(x => !existingAccountUsers.Select(y => y.AccountID).Contains(x)).ToList();

            var allInDatabase = dbContext.AccountUsers;

            existingAccountUsers.Merge(newAccountUsers, allInDatabase, (x, y) => x.UserID == y.UserID && x.AccountID == y.AccountID);

            dbContext.SaveChanges();

            return addedAccountIDs;
        }

        public static void RemoveAssociatedAccount(RioDbContext dbContext, int userID, int accountID)
        {
            var currentAccountUser = dbContext.AccountUsers.Single(x => x.UserID == userID && x.AccountID == accountID);
            dbContext.AccountUsers.Remove(currentAccountUser);
            dbContext.SaveChanges();
        }

        public static bool CheckIfUsersAreAdministrators(RioDbContext dbContext, List<int> userIDs)
        {
            return dbContext.Users.Any(x => userIDs.Contains(x.UserID) && x.RoleID == (int) RoleEnum.Admin);
        }
    }
}