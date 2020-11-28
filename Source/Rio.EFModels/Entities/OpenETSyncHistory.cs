using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Rio.API.Util;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncHistory
    {
        public static void New(RioDbContext dbContext, string startDateString, string endDateString)
        {
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);
            var yearsBeingUpdated = String.Join(", ",
                Enumerable.Range(0, (endDate.Year-startDate.Year) + 1)
                    .Select(d => startDate.AddYears(d).Year.ToString()));

            var openETSyncHistoryToAdd = new OpenETSyncHistory()
            {
                OpenETSyncResultTypeID = (int) OpenETSyncResultTypeEnum.InProgress,
                YearsInUpdateSeparatedByComma = yearsBeingUpdated,
                LastUpdatedDate = DateTime.UtcNow
            };

            dbContext.OpenETSyncHistory.Add(openETSyncHistoryToAdd);
            dbContext.SaveChanges();
        }

        public static void UpdateAnyInProgress(RioDbContext rioDbContext, OpenETSyncResultTypeEnum resultType)
        {
            var inProgressUpdates = rioDbContext.OpenETSyncHistory.Where(x =>
                x.OpenETSyncResultTypeID == (int) OpenETSyncResultTypeEnum.InProgress).ToList();

            if (!inProgressUpdates.Any())
            {
                return;
            }
            
            inProgressUpdates.ForEach(x =>
            {
                x.LastUpdatedDate = DateTime.Now;
                x.OpenETSyncResultTypeID = (int) resultType;
            });

            rioDbContext.SaveChanges();
        }

        public static object GetInProgress(RioDbContext dbContext)
        {
            return dbContext.OpenETSyncHistory
                .Include(x => x.OpenETSyncResultType)
                .SingleOrDefault(x => x.OpenETSyncResultTypeID == (int) OpenETSyncResultTypeEnum.InProgress)?.AsDto();
        }
    }
}
