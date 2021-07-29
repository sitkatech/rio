using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncHistory
    {
        public static OpenETSyncHistoryDto New(RioDbContext dbContext, int waterYearMonthID)
        {
            var waterYearMonth = dbContext.WaterYearMonths.Single(x => x.WaterYearMonthID == waterYearMonthID);
            
            var openETSyncHistoryToAdd = new OpenETSyncHistory()
            {
                OpenETSyncResultTypeID = (int)OpenETSyncResultTypeEnum.Created,
                WaterYearMonthID = waterYearMonthID,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            dbContext.OpenETSyncHistories.Add(openETSyncHistoryToAdd);
            dbContext.SaveChanges();
            dbContext.Entry(openETSyncHistoryToAdd).Reload();

            return GetByOpenETSyncHistoryID(dbContext, openETSyncHistoryToAdd.OpenETSyncHistoryID);
        }

        public static OpenETSyncHistoryDto GetByOpenETSyncHistoryID(RioDbContext dbContext, int openETSyncHistoryID)
        {
            return dbContext.OpenETSyncHistories
                .Include(x=>x.OpenETSyncResultType)
                .Include(x => x.WaterYearMonth)
                .ThenInclude(x => x.WaterYear)
                .SingleOrDefault(x => x.OpenETSyncHistoryID == openETSyncHistoryID).AsDto();
        }
        public static OpenETSyncHistoryDto UpdateOpenETSyncEntityByID(RioDbContext rioDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType)
        {
            return UpdateOpenETSyncEntityByID(rioDbContext, openETSyncHistoryID, resultType, null);
        }

        public static OpenETSyncHistoryDto UpdateOpenETSyncEntityByID(RioDbContext rioDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType, string errorMessage)
        {
            return UpdateOpenETSyncEntityByID(rioDbContext, openETSyncHistoryID, resultType, errorMessage, null);
        }

        public static OpenETSyncHistoryDto UpdateOpenETSyncEntityByID(RioDbContext rioDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType, string errorMessage, string googleBucketFileRetrievalURL)
        {
            var openETSyncHistory =
                rioDbContext.OpenETSyncHistories.Single(x => x.OpenETSyncHistoryID == openETSyncHistoryID);

            openETSyncHistory.UpdateDate = DateTime.UtcNow;
            openETSyncHistory.OpenETSyncResultTypeID = (int)resultType;
            if (resultType == OpenETSyncResultTypeEnum.Failed)
            {
                openETSyncHistory.ErrorMessage = errorMessage;
            }

            //Once this is set it should never change
            if (String.IsNullOrWhiteSpace(openETSyncHistory.GoogleBucketFileRetrievalURL))
            {
                openETSyncHistory.GoogleBucketFileRetrievalURL = googleBucketFileRetrievalURL;
            }
            
            rioDbContext.SaveChanges();
            rioDbContext.Entry(openETSyncHistory).Reload();

            return GetByOpenETSyncHistoryID(rioDbContext, openETSyncHistory.OpenETSyncHistoryID);
        }

        public static List<OpenETSyncHistoryDto> List(RioDbContext dbContext)
        {
            return dbContext.OpenETSyncHistories
                .Include(x => x.OpenETSyncResultType)
                .Include(x => x.WaterYearMonth)
                .ThenInclude(x => x.WaterYear)
                .OrderByDescending(x => x.CreateDate).Select(x => x.AsDto()).ToList();
        }
    }
}
