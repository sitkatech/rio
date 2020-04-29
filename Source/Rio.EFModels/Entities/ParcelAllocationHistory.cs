using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Account;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Rio.API.Util;
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
                ParcelAllocationTypeID = parcelAllocation.ParcelAllocationTypeID,
                UserID = userID,
                FileResourceID = fileResourceID,
                ParcelAllocationHistoryValue =
                    fileResourceID != null ? (decimal?)null : parcelAllocation.AcreFeetAllocated
            };

            dbContext.ParcelAllocationHistory.Add(parcelAllocationHistoryEntry);
            dbContext.SaveChanges();
        }

        public static void CreateParcelAllocationHistoryEntity(RioDbContext dbContext, int userID, int? fileResourceID, int waterYear, int parcelAllocationTypeID, decimal? allocated)
        {
            var parcelAllocationHistoryEntry = new ParcelAllocationHistory()
            {
                ParcelAllocationHistoryDate = DateTime.Now,
                ParcelAllocationHistoryWaterYear = waterYear,
                ParcelAllocationTypeID = parcelAllocationTypeID,
                UserID = userID,
                FileResourceID = fileResourceID,
                ParcelAllocationHistoryValue =
                    fileResourceID != null ? (decimal?)null : allocated
            };

            dbContext.ParcelAllocationHistory.Add(parcelAllocationHistoryEntry);
            dbContext.SaveChanges();
        }

        public static IEnumerable<ParcelAllocationHistoryDto> GetParcelAllocationHistoryDtos(RioDbContext dbContext)
        {
            return dbContext.ParcelAllocationHistory
                .Include(x => x.User)
                .Include(x => x.ParcelAllocationType)
                .Include(x => x.FileResource)
                .ToList().Select(x => new ParcelAllocationHistoryDto()
                {
                    Date = x.ParcelAllocationHistoryDate,
                    WaterYear = x.ParcelAllocationHistoryWaterYear,
                    Allocation = x.ParcelAllocationType.ParcelAllocationTypeDisplayName,
                    Value = x.ParcelAllocationHistoryValue,
                    Filename = x.FileResource?.OriginalBaseFilename,
                    User = x.User.FirstName + " " + x.User.LastName
                });
        }
    }
}