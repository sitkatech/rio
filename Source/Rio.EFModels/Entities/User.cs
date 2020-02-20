﻿using System;
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
                IsActive = true,
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
            // right now we are assuming a parcel can only be associated to one user
            var parcels = dbContext.vUserDetailed.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList()
                .Select(user =>
                {
                    var userDetailedDto = new UserDetailedDto()
                    {
                        UserID = user.UserID,
                        UserGuid = user.UserGuid,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        LoginName = user.LoginName,
                        RoleID = user.RoleID,
                        RoleDisplayName = user.RoleDisplayName,
                        Phone = user.Phone,
                        HasActiveTrades = user.HasActiveTrades,
                        AcreFeetOfWaterPurchased = user.AcreFeetOfWaterPurchased,
                        AcreFeetOfWaterSold = user.AcreFeetOfWaterSold,
                    };
                    return userDetailedDto;
                }).ToList();
            return parcels;
        }

        public static IEnumerable<UserDto> ListByRole(RioDbContext dbContext, RoleEnum roleEnum)
        {
            var users = GetUserImpl(dbContext)
                .Where(x => x.IsActive && x.RoleID == (int) roleEnum)
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return users;
        }

        public static IEnumerable<UserDto> AdminsThatReceiveSupportEmails(RioDbContext dbContext)
        {
            var users = GetUserImpl(dbContext)
                .Where(x => x.IsActive && x.RoleID == (int) RoleEnum.Admin && x.ReceiveSupportEmails)
                .Select(x => x.AsDto())
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

        public static UserDto SetAssociatedAccounts(RioDbContext dbContext, UserDto userDto, List<int> accountIDs, out List<int> addedAccountIDs)
        {
            var newAccountUsers = accountIDs.Select(accountID => new AccountUser() { UserID = userDto.UserID, AccountID = accountID }).ToList();

            var existingAccountUsers = dbContext.User.Include(x => x.AccountUser)
                .Single(x => x.UserID == userDto.UserID).AccountUser;

            addedAccountIDs = accountIDs.Where(x => !existingAccountUsers.Select(y => y.AccountID).Contains(x)).ToList();

            var allInDatabase = dbContext.AccountUser;

            existingAccountUsers.Merge(newAccountUsers, allInDatabase, (x, y) => x.UserID == y.UserID && x.AccountID == y.AccountID);

            dbContext.SaveChanges();

            return GetByUserID(dbContext, userDto.UserID);
        }
    }
}