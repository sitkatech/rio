using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.User;

namespace Rio.API.Services
{
    public class UserContext
    {
        public UserDto User { get; set; }

        private UserContext(UserDto user)
        {
            User = user;
        }

        public static UserDto GetUserFromHttpContext(RioDbContext dbContext, HttpContext httpContext)
        {

            var claimsPrincipal = httpContext.User;
            if (!claimsPrincipal.Claims.Any())
            {
                return null;
            }

            var userGuid = Guid.Parse(claimsPrincipal.Claims.Single(c => c.Type == "sub").Value);
            var keystoneUser = Rio.EFModels.Entities.User.GetSingle(dbContext, userGuid);
            return keystoneUser;
        }
    }
}