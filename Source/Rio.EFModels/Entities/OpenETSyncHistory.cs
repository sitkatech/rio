using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncHistory
    {
        public static OpenETSyncHistoryDto New(RioDbContext dbContext, string startDateString, string endDateString, string suffix)
        {
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);
            var yearsBeingUpdatedFullRange = Enumerable.Range(0, (endDate.Year - startDate.Year) + 1)
                .Select(d => startDate.AddYears(d).Year).ToList();
            
            //Because we know that the update function will automatically filter out finalized water years, exclude them from the history here 
            //even if the date range contains any finalized year.
            var finalizedYears = dbContext.OpenETSyncWaterYearStatus
                .Where(x => x.OpenETSyncStatusTypeID == (int) OpenETSyncStatusTypeEnum.Finalized)
                .Select(x => x.WaterYear).ToList();
            var yearsBeingUpdated = yearsBeingUpdatedFullRange.Where(x => !finalizedYears.Contains(x)).ToList();

            OpenETSyncWaterYearStatus.UpdateSyncStatusTypeByWaterYear(dbContext, yearsBeingUpdated, OpenETSyncStatusTypeEnum.CurrentlyUpdating);

            var yearsBeingUpdatedAsString = String.Join(",", yearsBeingUpdated);

            var openETSyncHistoryToAdd = new OpenETSyncHistory()
            {
                OpenETSyncResultTypeID = (int) OpenETSyncResultTypeEnum.InProgress,
                YearsInUpdateSeparatedByComma = yearsBeingUpdatedAsString,
                UpdatedFileSuffix = suffix,
                LastUpdatedDate = DateTime.UtcNow
            };

            dbContext.OpenETSyncHistory.Add(openETSyncHistoryToAdd);
            dbContext.SaveChanges();
            dbContext.Entry(openETSyncHistoryToAdd).Reload();

            return GetByOpenETSyncHistoryID(dbContext, openETSyncHistoryToAdd.OpenETSyncHistoryID);
        }

        public static OpenETSyncHistoryDto GetByOpenETSyncHistoryID(RioDbContext dbContext, int openETSyncHistoryID)
        {
            return dbContext.OpenETSyncHistory.Include(x=>x.OpenETSyncResultType).SingleOrDefault(x => x.OpenETSyncHistoryID == openETSyncHistoryID).AsDto();
        }

        public static List<OpenETSyncHistoryDto> ListInProgress(RioDbContext dbContext)
        {
            return dbContext.OpenETSyncHistory
                .Include(x => x.OpenETSyncResultType)
                .Where(x => x.OpenETSyncResultTypeID == (int) OpenETSyncResultTypeEnum.InProgress)?.Select(x => x.AsDto()).ToList();
        }

        public static void UpdateSyncResultByID(RioDbContext rioDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType)
        {
            var openETSyncHistory =
                rioDbContext.OpenETSyncHistory.Single(x => x.OpenETSyncHistoryID == openETSyncHistoryID);

            OpenETSyncWaterYearStatus.UpdateSyncStatusTypeByWaterYear(rioDbContext, openETSyncHistory.YearsInUpdateSeparatedByComma.Split(',').Select(Int32.Parse).ToList(), OpenETSyncStatusTypeEnum.Nightly);

            openETSyncHistory.LastUpdatedDate = DateTime.Now;
            openETSyncHistory.OpenETSyncResultTypeID = (int) resultType;

            rioDbContext.SaveChanges();
        }
    }
}
