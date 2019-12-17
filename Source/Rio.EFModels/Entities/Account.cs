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
        public static List<AccountDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            return dbContext.User.Include(x => x.AccountUser).ThenInclude(x => x.Account)
                .Single(x => x.UserID == userID).AccountUser.Select(x => x.Account.AsDto()).ToList();
        }
    }
}
