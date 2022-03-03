using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class AccountStatus
    {

        public static object List(RioDbContext dbContext)
        {
            var roles = dbContext.AccountStatuses
                .AsNoTracking()
                .Select(x => x.AsDto());

            return roles;
        }

        public static object GetByAccountStatusID(RioDbContext dbContext, int accountStatusID)
        {
            var accountStatus = dbContext.AccountStatuses
                .AsNoTracking()
                .FirstOrDefault(x => x.AccountStatusID == accountStatusID);

            return accountStatus?.AsDto();
        }
    }
}
