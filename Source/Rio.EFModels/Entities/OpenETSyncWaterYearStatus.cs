using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Rio.API.Util;
using Rio.Models.DataTransferObjects;

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

        public static List<OpenETSyncWaterYearStatusDto> List(RioDbContext dbContext)
        {
            return dbContext.OpenETSyncWaterYearStatus
                .Include(x => x.OpenETSyncStatusType)
                .OrderByDescending(x => x.WaterYear)
                .Select(x => x.AsDto())
                .ToList();
        }

        public static OpenETSyncWaterYearStatusDto GetByOpenETSyncWaterYearStatusID(RioDbContext dbContext, int openETSyncWaterYearStatusId)
        {
            return dbContext.OpenETSyncWaterYearStatus
                .Include(x => x.OpenETSyncStatusType)
                .SingleOrDefault(x => x.OpenETSyncWaterYearStatusID == openETSyncWaterYearStatusId).AsDto();
        }

        public static OpenETSyncWaterYearStatusDto Finalize(RioDbContext dbContext, int openETSyncWaterYearStatusId)
        {
            var openETSyncWaterYearStatus =
                dbContext.OpenETSyncWaterYearStatus.Single(x =>
                    x.OpenETSyncWaterYearStatusID == openETSyncWaterYearStatusId);

            openETSyncWaterYearStatus.OpenETSyncStatusTypeID = (int) OpenETSyncStatusTypeEnum.Finalized;
            openETSyncWaterYearStatus.LastUpdatedDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(openETSyncWaterYearStatus).Reload();

            return GetByOpenETSyncWaterYearStatusID(dbContext, openETSyncWaterYearStatusId);
        }

        public static void UpdateSyncStatusTypeByYear(RioDbContext dbContext, List<int> yearsBeingUpdated, OpenETSyncStatusTypeEnum newStatus)
        {
            var openETSyncWaterYearStatus = dbContext.OpenETSyncWaterYearStatus
                .Where(x => yearsBeingUpdated.Contains(x.WaterYear)).ToList();

            if (!openETSyncWaterYearStatus.Any())
            {
                return;
            }

            openETSyncWaterYearStatus.ForEach(x => x.OpenETSyncStatusTypeID = (int)newStatus);
            dbContext.SaveChanges();
        }
    }
}
