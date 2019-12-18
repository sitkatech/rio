using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Account;

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
            return dbContext.Account.Include(x => x.AccountUser).ThenInclude(x => x.User).Select(x => x.AsDto())
                .ToList();
        }
    }
}
