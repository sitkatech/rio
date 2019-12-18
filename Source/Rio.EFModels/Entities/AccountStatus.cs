using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class AccountStatus
    {

        public static object List(RioDbContext dbContext)
        {
            var roles = dbContext.AccountStatus
                .AsNoTracking()
                .Select(x => x.AsDto());

            return roles;
        }
    }
}
