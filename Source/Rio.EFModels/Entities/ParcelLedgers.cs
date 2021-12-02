using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
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
                .Include(x => x.Parcel)
                .ThenInclude(x => x.ParcelStatus)
                .AsNoTracking();
        }

        private static IQueryable<ParcelLedger> GetAllocationsImpl(RioDbContext dbContext)
        {
            return GetParcelLedgersImpl(dbContext)
                .Where(x => x.TransactionTypeID == (int) TransactionTypeEnum.Supply && x.ParcelLedgerEntrySourceTypeID == (int) ParcelLedgerEntrySourceTypeEnum.Manual);
        }

        public static List<ParcelAllocationBreakdownDto> GetParcelAllocationBreakdownForYearAsDto(RioDbContext dbContext, int year)
        {
            var parcelAllocationBreakdownForYear = GetAllocationsImpl(dbContext)
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

            var parcelAllocations = GetAllocationsImpl(dbContext)
                .Where(x => x.EffectiveDate.Year == year);
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

        public static List<ParcelLedgerDto> ListByAccountID(RioDbContext dbContext, int accountID)
        {
            var parcelIDs = Parcel.ListByAccountID(dbContext, accountID)
                .Select(x => x.ParcelID);

            var parcelLedgerDtos = ListByParcelIDAsDto(dbContext, parcelIDs).OrderByDescending(x => x.EffectiveDate)
                .ToList();

            return parcelLedgerDtos;
        }

        public static List<ParcelLedgerDto> ListByParcelIDAsDto(RioDbContext dbContext, IEnumerable<int> parcelIDs)
        {
            var parcelLedgers = GetParcelLedgersImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID))
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.EffectiveDate)
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

        public static ParcelLedgerDto CreateNew(RioDbContext dbContext, ParcelLedgerCreateDto parcelLedgerCreateDto, int userID)
        {
            var parcelLedger = new ParcelLedger
            {
                ParcelID = parcelLedgerCreateDto.ParcelID,
                TransactionDate = DateTime.UtcNow,
                EffectiveDate = parcelLedgerCreateDto.EffectiveDate,
                TransactionTypeID = parcelLedgerCreateDto.TransactionTypeID,
                TransactionAmount = (parcelLedgerCreateDto.IsWithdrawal ? -parcelLedgerCreateDto.TransactionAmount : parcelLedgerCreateDto.TransactionAmount),
                WaterTypeID = parcelLedgerCreateDto.WaterTypeID,
                TransactionDescription = 
                    $"A manual {(parcelLedgerCreateDto.IsWithdrawal ? "withdrawal from" : "deposit to")} water {(parcelLedgerCreateDto.WaterTypeID.HasValue ? "supply" : "usage")} has been applied to this water account.",
                UserID = userID,
                UserComment = parcelLedgerCreateDto.UserComment
            };

            dbContext.ParcelLedgers.Add(parcelLedger);
            dbContext.SaveChanges();
            dbContext.Entry(parcelLedger).Reload();

            return GetByIDAsDto(dbContext, parcelLedger.ParcelLedgerID);
        }
    }
}