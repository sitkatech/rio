using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public static IEnumerable<UserDto> List(RioDbContext dbContext)
        {
            var users = dbContext.User
                .Include(x => x.Role)
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Role.RoleID).ThenBy(x => x.FirstName).ThenBy(x => x.LastName)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return users;
        }

        public static UserDto GetByUserID(RioDbContext dbContext, int userID)
        {
            var user = dbContext.User
                .Include(x => x.Role)
                .AsNoTracking()
                .SingleOrDefault(x => x.UserID == userID);

            var userDto = user?.AsDto();
            return userDto;
        }

        public static UserDto GetByUserGuid(RioDbContext dbContext, Guid userGuid)
        {
            var user = dbContext.User
                .Include(x => x.Role)
                .AsNoTracking()
                .SingleOrDefault(x => x.UserGuid == userGuid);

            var userDto = user?.AsDto();
            return userDto;
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
            user.UpdateDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(user).Reload();
            return GetByUserID(dbContext, userID);
        }

        public static List<ErrorMessage> ValidateUpdate(RioDbContext dbContext, UserUpsertDto userEditDto, int userID)
        {
            var result = new List<ErrorMessage>();

            //if (string.IsNullOrWhiteSpace(userEditDto.FirstName))
            //{
            //    result.Add(new ErrorMessage() { Type = "First Name", Message = "First Name is required." });
            //}

            //if (string.IsNullOrWhiteSpace(userEditDto.LastName))
            //{
            //    result.Add(new ErrorMessage() { Type = "Last Name", Message = "Last Name is required." });
            //}

            if (!userEditDto.RoleID.HasValue)
            {
                result.Add(new ErrorMessage() { Type = "Role ID", Message = "Role ID is required." });
            }

            return result;
        }
    }
}