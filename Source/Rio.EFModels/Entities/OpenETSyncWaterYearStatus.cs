using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Rio.API.Util;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncWaterYearStatus
    {
        public static void UpdateUpdatedDateAndAddIfNecessary(RioDbContext rioDbContext, List<int> waterYearsUpdated)
        {
            var toUpdateOrAdd = waterYearsUpdated.Select(x => new OpenETSyncWaterYearStatus()
            {
                LastUpdatedDate = DateTime.UtcNow,
                OpenETSyncStatusTypeID = (int) OpenETSyncStatusTypeEnum.Nightly,
                WaterYear = x
            });

            var existingOpenETSyncWaterYearStatus =
                rioDbContext.OpenETSyncWaterYearStatus.Where(x => waterYearsUpdated.Contains(x.WaterYear)).ToList();

            var allInDatabase = rioDbContext.OpenETSyncWaterYearStatus;

            existingOpenETSyncWaterYearStatus.MergeUpdate(toUpdateOrAdd, ((x, y) => x.WaterYear == y.WaterYear), ((x, y) => x.LastUpdatedDate = y.LastUpdatedDate));

            existingOpenETSyncWaterYearStatus.MergeNew(toUpdateOrAdd, allInDatabase, ((x, y) => x.WaterYear == y.WaterYear));

            rioDbContext.SaveChanges();
        }
    }
}
