using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class AccountParcelWaterYear
    {
        public static AccountParcelWaterYear GetByParcelIDAndWaterYearID(RioDbContext dbContext, int parcelID, int waterYearID)
        {
            return dbContext.AccountParcelWaterYears.AsNoTracking()
                .SingleOrDefault(x => x.ParcelID == parcelID && x.WaterYearID == waterYearID);
        }

        public static void ChangeParcelOwnerForWaterYears(RioDbContext dbContext, int parcelId, IEnumerable<int> waterYearsToUpdate, int? accountId)
        {
            var currentAccountParcelWaterYearRecords = dbContext.AccountParcelWaterYears.Where(x =>
                x.ParcelID == parcelId && waterYearsToUpdate.Contains(x.WaterYearID));

            dbContext.AccountParcelWaterYears.RemoveRange(currentAccountParcelWaterYearRecords);

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
                    dbContext.AccountParcelWaterYears.Add(newAccountParcelWaterYearAssociation);
                }
            }

            dbContext.SaveChanges();
        }
    }
}