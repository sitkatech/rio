using System.Collections.Generic;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class AccountParcelWaterYear
    {
        public static void ChangeParcelOwnerForWaterYears(RioDbContext dbContext, int parcelId, IEnumerable<int> waterYearsToUpdate, int? accountId)
        {
            var currentAccountParcelWaterYearRecords = dbContext.AccountParcelWaterYear.Where(x =>
                x.ParcelID == parcelId && waterYearsToUpdate.Contains(x.WaterYearID));

            dbContext.AccountParcelWaterYear.RemoveRange(currentAccountParcelWaterYearRecords);

            if (accountId.HasValue)
            {
                foreach (var waterYearID in waterYearsToUpdate)
                {
                    var newAccountParcelWaterYearAssociation = new AccountParcelWaterYear
                    {
                        AccountID = accountId.Value,
                        ParcelID = parcelId,
                        WaterYearID = waterYearID
                    };
                    dbContext.AccountParcelWaterYear.Add(newAccountParcelWaterYearAssociation);
                }
            }

            dbContext.SaveChanges();
        }
    }
}