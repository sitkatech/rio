using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncHistory
    {
        public static OpenETSyncHistoryDto New(RioDbContext dbContext, int year)
        {
            var openETSyncHistoryToAdd = new OpenETSyncHistory()
            {
                OpenETSyncResultTypeID = (int)OpenETSyncResultTypeEnum.InProgress,
                WaterYearID = dbContext.WaterYear.Single(x => x.Year == year).WaterYearID,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            dbContext.OpenETSyncHistory.Add(openETSyncHistoryToAdd);
            dbContext.SaveChanges();
            dbContext.Entry(openETSyncHistoryToAdd).Reload();

            return GetByOpenETSyncHistoryID(dbContext, openETSyncHistoryToAdd.OpenETSyncHistoryID);
        }

        public static OpenETSyncHistoryDto GetByOpenETSyncHistoryID(RioDbContext dbContext, int openETSyncHistoryID)
        {
            return dbContext.OpenETSyncHistory
                .Include(x=>x.OpenETSyncResultType)
                .Include(x => x.WaterYear)
                .SingleOrDefault(x => x.OpenETSyncHistoryID == openETSyncHistoryID).AsDto();
        }

        public static List<OpenETSyncHistoryDto> ListInProgress(RioDbContext dbContext)
        {
            return dbContext.OpenETSyncHistory
                .Include(x => x.OpenETSyncResultType)
                .Include(x => x.WaterYear)
                .Where(x => x.OpenETSyncResultTypeID == (int) OpenETSyncResultTypeEnum.InProgress)?.Select(x => x.AsDto()).ToList();
        }

        public static OpenETSyncHistoryDto UpdateSyncResultByID(RioDbContext rioDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType)
        {
            var openETSyncHistory =
                rioDbContext.OpenETSyncHistory.Single(x => x.OpenETSyncHistoryID == openETSyncHistoryID);

            openETSyncHistory.UpdateDate = DateTime.UtcNow;
            openETSyncHistory.OpenETSyncResultTypeID = (int) resultType;

            rioDbContext.SaveChanges();
            rioDbContext.Entry(openETSyncHistory).Reload();

            return GetByOpenETSyncHistoryID(rioDbContext, openETSyncHistory.OpenETSyncHistoryID);
        }

        public static List<OpenETSyncHistoryDto> List(RioDbContext dbContext)
        {
            return dbContext.OpenETSyncHistory
                .Include(x => x.OpenETSyncResultType)
                .Include(x => x.WaterYear)
                .OrderByDescending(x => x.CreateDate).Select(x => x.AsDto()).ToList();
        }
    }
}
