using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocationHistory
    {
        public static void CreateParcelAllocationHistoryEntity(RioDbContext dbContext, int userID,
            ParcelAllocationUpsertDto parcelAllocation, int? fileResourceID)
        {
            var parcelAllocationHistoryEntry = new ParcelAllocationHistory()
            {
                ParcelAllocationHistoryDate = DateTime.Now,
                ParcelAllocationHistoryWaterYear = parcelAllocation.WaterYear,
                WaterTypeID = parcelAllocation.WaterTypeID,
                UserID = userID,
                FileResourceID = fileResourceID,
                ParcelAllocationHistoryValue =
                    fileResourceID != null ? (decimal?)null : parcelAllocation.AcreFeetAllocated
            };

            dbContext.ParcelAllocationHistories.Add(parcelAllocationHistoryEntry);
            dbContext.SaveChanges();
        }

        public static void CreateParcelAllocationHistoryEntity(RioDbContext dbContext, int userID, int? fileResourceID, int waterYear, int waterTypeID, decimal? allocated)
        {
            var parcelAllocationHistoryEntry = new ParcelAllocationHistory()
            {
                ParcelAllocationHistoryDate = DateTime.Now,
                ParcelAllocationHistoryWaterYear = waterYear,
                WaterTypeID = waterTypeID,
                UserID = userID,
                FileResourceID = fileResourceID,
                ParcelAllocationHistoryValue =
                    fileResourceID != null ? (decimal?)null : allocated
            };

            dbContext.ParcelAllocationHistories.Add(parcelAllocationHistoryEntry);
            dbContext.SaveChanges();
        }

        public static IEnumerable<ParcelAllocationHistoryDto> GetParcelAllocationHistoryDtos(RioDbContext dbContext)
        {
            return dbContext.ParcelAllocationHistories
                .Include(x => x.User).ThenInclude(x => x.Role)
                .Include(x => x.WaterType)
                .Include(x => x.FileResource).ThenInclude(x => x.FileResourceMimeType)
                .Select(x => x.AsDto()).ToList();
        }
    }
}