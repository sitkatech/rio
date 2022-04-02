using System.Linq;

namespace Rio.EFModels.Entities
{
    public static class AccountStatuses
    {

        public static object List(RioDbContext dbContext)
        {
            return AccountStatus.AllAsDto;
        }

        public static object GetByAccountStatusID(RioDbContext dbContext, int accountStatusID)
        {
            return AccountStatus.AllAsDtoLookupDictionary[accountStatusID];
        }
    }
}
