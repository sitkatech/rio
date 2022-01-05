using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public static class ParcelLedgers
    {
        private static IQueryable<ParcelLedger> GetParcelLedgersImpl(RioDbContext dbContext)
        {
            return dbContext.ParcelLedgers
                .Include(x => x.TransactionType)
                .Include(x => x.WaterType)
                .Include(x => x.ParcelLedgerEntrySourceType)
                .Include(x => x.Parcel).ThenInclude(x => x.ParcelStatus)
                .AsNoTracking();
        }

        public static IQueryable<ParcelLedger> GetSupplyByParcelLedgerEntrySourceType(RioDbContext dbContext, List<ParcelLedgerEntrySourceTypeEnum> parcelLedgerEntrySourceTypeEnums)
        {
            var parcelLedgerEntrySourceTypeIDs = parcelLedgerEntrySourceTypeEnums.Select(x => (int)x).ToList();
            return GetParcelLedgersImpl(dbContext).Where(x => x.TransactionTypeID == (int) TransactionTypeEnum.Supply &&
                            parcelLedgerEntrySourceTypeIDs.Contains(x.ParcelLedgerEntrySourceTypeID));
        }

        private static IQueryable<ParcelLedger> GetSupplyImpl(RioDbContext dbContext)
        {
            return GetSupplyByParcelLedgerEntrySourceType(dbContext, new List<ParcelLedgerEntrySourceTypeEnum>{ ParcelLedgerEntrySourceTypeEnum.Manual, ParcelLedgerEntrySourceTypeEnum.CIMIS });
        }

        public static List<ParcelAllocationBreakdownDto> GetParcelAllocationBreakdownForYearAsDto(RioDbContext dbContext, int year)
        {
            var parcelAllocationBreakdownForYear = GetSupplyImpl(dbContext)
                .Where(x => x.EffectiveDate.Year == year && x.WaterTypeID != null)
                .ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => new ParcelAllocationBreakdownDto
                {
                    ParcelID = x.Key,
                    // There's at most one ParcelAllocation per Parcel per WaterType, so we just need to read elements of the group into this dictionary
                    Allocations = x.Where(y => y.WaterTypeID.HasValue).GroupBy(y => y.WaterTypeID.Value).ToDictionary(y => y.Key, y => y.Sum(z => z.TransactionAmount))
                }).ToList();
            return parcelAllocationBreakdownForYear;
        }

        public static decimal GetUsageSumForMonthAndParcelID(RioDbContext dbContext, int year, int month, int parcelID)
        {
            return GetUsagesByParcelIDs(dbContext, new List<int>(parcelID))
                    .Where(x => x.EffectiveDate.Year == year && x.EffectiveDate.Month == month)
                    .Sum(x => x.TransactionAmount);
        }

        public static IQueryable<ParcelLedger> GetUsagesByParcelIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            return GetParcelLedgersImpl(dbContext).Where(x => x.TransactionTypeID == (int) TransactionTypeEnum.Usage);
        }

        public static List<LandownerAllocationBreakdownDto> GetLandownerAllocationBreakdownForYear(RioDbContext dbContext, int year)
        {
            var accountParcelWaterYearOwnershipsByYear = Entities.Parcel.AccountParcelWaterYearOwnershipsByYear(dbContext, year);

            var parcelAllocations = GetSupplyImpl(dbContext)
                .Where(x => x.EffectiveDate.Year == year && x.WaterTypeID != null);
            if (parcelAllocations.Any())
            {
                return accountParcelWaterYearOwnershipsByYear
                    .GroupJoin(
                        parcelAllocations,
                        x => x.ParcelID,
                        y => y.ParcelID,
                        (x, y) => new
                        {
                            ParcelOwnership = x,
                            ParcelAllocation = y
                        })
                    .SelectMany(
                        parcelOwnershipAndAllocations =>
                            parcelOwnershipAndAllocations.ParcelAllocation.DefaultIfEmpty(),
                        (x, y) => new
                        {
                            x.ParcelOwnership.AccountID,
                            WaterTypeID = y.WaterTypeID.Value,
                            y.TransactionAmount
                        })
                    .ToList()
                    .GroupBy(x => x.AccountID)
                    .Select(x => new LandownerAllocationBreakdownDto()
                    {
                        AccountID = x.Key,
                        Allocations = new Dictionary<int, decimal>(
                            //unlike above, there may be many ParcelAllocations per Account per Allocation Type, so we need an additional grouping.
                            x.GroupBy(z => z.WaterTypeID)
                                .Select(y =>
                                    new KeyValuePair<int, decimal>(y.Key,
                                        y.Sum(x => x.TransactionAmount))))
                    })
                    .ToList();
            }

            return new List<LandownerAllocationBreakdownDto>();
        }

        public static List<ParcelLedgerDto> ListByAccountIDForAllWaterYears(RioDbContext dbContext, int accountID)
        {
            var parcelLedgerDtos = new List<ParcelLedgerDto>();
            var parcelIDsForYearGroups = Parcel.AccountParcelWaterYearOwnerships(dbContext)
                .Where(x => x.AccountID == accountID)
                .AsEnumerable()
                .GroupBy(x => x.WaterYear.Year);

            foreach (var group in parcelIDsForYearGroups)
            {
                var parcelIDsForYear = group.Select(x => x.ParcelID).ToList();
                parcelLedgerDtos.AddRange(GetParcelLedgersImpl(dbContext)
                    .Where(x => parcelIDsForYear.Contains(x.ParcelID) && x.EffectiveDate.Year == group.Key)
                    .OrderByDescending(x => x.EffectiveDate)
                    .ThenByDescending(x => x.TransactionDate)
                    .Select(x => x.AsDto()));
            }

            return parcelLedgerDtos;
        }

        public static List<ParcelLedgerDto> ListByParcelIDAsDto(RioDbContext dbContext, IEnumerable<int> parcelIDs)
        {
            var parcelLedgers = GetParcelLedgersImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID))
                .OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.TransactionDate)
                .Select(x => x.AsDto())
                .ToList();

            return parcelLedgers;
        }

        public static List<ParcelLedgerDto> ListByParcelIDAsDto(RioDbContext dbContext, int parcelID)
        {
            var parcelLedgers = ListByParcelIDAsDto(dbContext, new List<int> {parcelID});
            return parcelLedgers;
        }

        public static ParcelLedgerDto GetByIDAsDto(RioDbContext dbContext, int parcelLedgerID)
        {
            var parcelLedger = GetParcelLedgersImpl(dbContext).SingleOrDefault(x => x.ParcelLedgerID == parcelLedgerID);
            return parcelLedger?.AsDto();
        }

        public static void CreateNew(RioDbContext dbContext, ParcelDto parcel, ParcelLedgerCreateDto parcelLedgerCreateDto, int userID)
        {
            foreach (var parcelNumber in parcelLedgerCreateDto.ParcelNumbers)
            {
                var parcelLedger = new ParcelLedger()
                {
                        ParcelID = parcel.ParcelID,
                        TransactionDate = DateTime.UtcNow,
                        EffectiveDate = parcelLedgerCreateDto.EffectiveDate.AddHours(8),
                        TransactionTypeID = parcelLedgerCreateDto.TransactionTypeID,
                        ParcelLedgerEntrySourceTypeID = (int)ParcelLedgerEntrySourceTypeEnum.Manual,
                        TransactionAmount = parcelLedgerCreateDto.TransactionAmount,
                        WaterTypeID = parcelLedgerCreateDto.WaterTypeID,
                        TransactionDescription =
                            $"A manual {(parcelLedgerCreateDto.TransactionAmount < 0 ? "withdrawal from" : "deposit to")} water {(parcelLedgerCreateDto.WaterTypeID.HasValue ? "supply" : "usage")} has been applied to this water account.",
                        UserID = userID,
                        UserComment = parcelLedgerCreateDto.UserComment
                };
                dbContext.ParcelLedgers.Add(parcelLedger);
            }
            dbContext.SaveChanges();
        }

        public static int BulkCreateNew(RioDbContext dbContext, ParcelLedgerCreateDto parcelLedgerCreateDto, int userID)
        {
            int createdCount = 0;
            var parcels = Parcel.ListByParcelNumbers(dbContext, parcelLedgerCreateDto.ParcelNumbers);
            
            foreach (var parcel in parcels)
            {
                var parcelLedger = new ParcelLedger()
                {
                    ParcelID = parcel.ParcelID,
                    TransactionDate = DateTime.UtcNow,
                    EffectiveDate = parcelLedgerCreateDto.EffectiveDate.AddHours(8),
                    TransactionTypeID = parcelLedgerCreateDto.TransactionTypeID,
                    ParcelLedgerEntrySourceTypeID = (int) ParcelLedgerEntrySourceTypeEnum.Manual,
                    TransactionAmount = parcelLedgerCreateDto.TransactionAmount * (decimal) parcel.ParcelAreaInAcres,
                    WaterTypeID = parcelLedgerCreateDto.WaterTypeID,
                    TransactionDescription =
                        $"A manual {(parcelLedgerCreateDto.TransactionAmount < 0 ? "withdrawal from" : "deposit to")} water supply has been applied to this water account.",
                    UserID = userID,
                    UserComment = parcelLedgerCreateDto.UserComment
                };
                dbContext.ParcelLedgers.Add(parcelLedger);
                createdCount++;
            }
            dbContext.SaveChanges();

            return createdCount;
        }

        public static int CreateNewFromCSV(RioDbContext dbContext, List<ParcelLedgerCreateCSV> records, string uploadedFileName, DateTime effectiveDate, int waterTypeID, int userID)
        {
            int createdCount = 0;
            var parcelNumbers = records.Select(x => x.APN).ToList();
            var parcels = Parcel.ListByParcelNumbers(dbContext, parcelNumbers);

            foreach (var record in records)
            {
                var parcel = parcels.SingleOrDefault(x => x.ParcelNumber == record.APN);
                var parcelLedger = new ParcelLedger()
                {
                    ParcelID = parcel.ParcelID,
                    TransactionDate = DateTime.UtcNow,
                    EffectiveDate = effectiveDate.AddHours(8),
                    TransactionTypeID = (int)TransactionTypeEnum.Supply,
                    ParcelLedgerEntrySourceTypeID = (int)ParcelLedgerEntrySourceTypeEnum.Manual,
                    TransactionAmount = (decimal)record.Quantity,
                    WaterTypeID = waterTypeID,
                    TransactionDescription =
                        $"Transaction recorded via spreadsheet upload: {uploadedFileName}",
                    UserID = userID
                };
                dbContext.ParcelLedgers.Add(parcelLedger);
                createdCount++;
            }
            dbContext.SaveChanges();

            return createdCount;
        }
    }
}