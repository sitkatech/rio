using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class Roles
    {
        public static IEnumerable<RoleDto> List(RioDbContext dbContext)
        {
            return Role.All.Select(x => x.AsDto());
        }

        public static RoleDto GetByRoleID(RioDbContext dbContext, int roleID)
        {
            return Role.AllLookupDictionary[roleID]?.AsDto();
        }
    }
}
