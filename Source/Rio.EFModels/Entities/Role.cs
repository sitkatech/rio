using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Role;

namespace Rio.EFModels.Entities
{
    public partial class Role
    {
        public static IEnumerable<RoleDto> GetList(RioDbContext dbContext)
        {
            var roles = dbContext.Role
                .AsNoTracking()
                .Select(x => x.AsDto());

            return roles;
        }

        public static RoleDto GetSingle(RioDbContext dbContext, int roleID)
        {
            var role = dbContext.Role
                .AsNoTracking()
                .FirstOrDefault(x => x.RoleID == roleID);

            return role?.AsDto();
        }
    }

    public enum RoleEnum
    {
        Admin = 1,
        Unassigned = 2,
        Normal = 3,
        SitkaAdmin = 4
    }
}
