using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.API.Util;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLedger
    {
        private const int TransactionTypeIDMeasuredUsageCorrection = 18;
        private const int TransactionTypeIDMeasuredUsage = 17;

        public static List<ParcelLedgerDto> ListAllocationsByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelLedgers = GetAllocationsImpl(dbContext)
                .Where(x => x.ParcelID == parcelID);

            return parcelLedgers.Any()
                ? parcelLedgers.Select(x => x.AsDto()).ToList()
                : new List<ParcelLedgerDto>();
        }

        private static IQueryable<ParcelLedger> GetAllocationsImpl(RioDbContext dbContext)
        {
            return dbContext.ParcelLedgers.Include(x => x.TransactionType)
                .AsNoTracking()
                .Where(x => x.TransactionType.IsAllocation);
        }

        public static List<ParcelLedgerDto> ListAllocationsByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelLedgers = GetAllocationsImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID));

            return parcelLedgers.Any()
                ? parcelLedgers.Select(x => x.AsDto()).ToList()
                : new List<ParcelLedgerDto>();
        }

        public static List<ParcelAllocationBreakdownDto> GetParcelAllocationBreakdownForYear(RioDbContext dbContext, int year)
        {
            var parcelAllocationBreakdownForYear = GetAllocationsImpl(dbContext)
                .Where(x => x.TransactionDate.Year == year)
                .ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => new ParcelAllocationBreakdownDto
                {
                    ParcelID = x.Key,
                    // There's at most one ParcelAllocation per Parcel per AllocationType, so we just need to read elements of the group into this dictionary
                    Allocations = new Dictionary<int, decimal>(x.Select(y =>
                        new KeyValuePair<int, decimal>(y.TransactionTypeID,
                            y.TransactionAmount)))
                }).ToList();
            return parcelAllocationBreakdownForYear;
        }
        public static List<ParcelMonthlyEvapotranspirationDto> ListMonthlyEvapotranspirationsByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelLedgerMeasuredUsages = dbContext.ParcelLedgers.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionTypeID == TransactionTypeIDMeasuredUsage);

            var parcelMonthlyEvapotranspirationDtos = parcelLedgerMeasuredUsages.Select(x => 
                new ParcelMonthlyEvapotranspirationDto()
                {
                    ParcelID = x.ParcelID,
                    WaterYear = x.TransactionDate.Year,
                    WaterMonth = x.TransactionDate.Month,
                    EvapotranspirationRate = -x.TransactionAmount
                }
            ).ToList();
            
            var parcelLedgerMeasuredUsageCorrections = dbContext.ParcelLedgers.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionTypeID == TransactionTypeIDMeasuredUsageCorrection);

            var parcelMonthlyEvapotranspirationCorrectionDtos = parcelLedgerMeasuredUsageCorrections.Select(x => 
                new ParcelMonthlyEvapotranspirationDto()
                {
                    ParcelID = x.ParcelID,
                    WaterYear = x.TransactionDate.Year,
                    WaterMonth = x.TransactionDate.Month,
                    OverriddenEvapotranspirationRate = -x.TransactionAmount
                }
            ).ToList();
            foreach (var parcelMonthlyEvapotranspirationDto in parcelMonthlyEvapotranspirationCorrectionDtos)
            {
                var existingParcelMonthlyEvapotranspirationDto = parcelMonthlyEvapotranspirationDtos.SingleOrDefault(y =>
                    y.ParcelID == parcelMonthlyEvapotranspirationDto.ParcelID && y.WaterMonth == parcelMonthlyEvapotranspirationDto.WaterMonth && y.WaterYear == parcelMonthlyEvapotranspirationDto.WaterYear);
                if (existingParcelMonthlyEvapotranspirationDto == null)
                {
                    parcelMonthlyEvapotranspirationDtos.Add(parcelMonthlyEvapotranspirationDto);
                }
                else
                {
                    existingParcelMonthlyEvapotranspirationDto.OverriddenEvapotranspirationRate = parcelMonthlyEvapotranspirationDto.OverriddenEvapotranspirationRate;
                }
            }

            return parcelMonthlyEvapotranspirationDtos;
        }
        public static List<ParcelMonthlyEvapotranspirationDto> ListMonthlyEvapotranspirationsByParcelIDAndYear(RioDbContext dbContext, List<int> parcelIDs,
      List<ParcelDto> parcels, int year)
        {
            var parcelMonthlyEvapotranspirations = new List<ParcelMonthlyEvapotranspirationDto>();
            // make the full matrix of months * parcels and populate with zero/empty
            foreach (var parcel in parcels)
            {
                for (var i = 1; i < 13; i++)
                {
                    parcelMonthlyEvapotranspirations.Add(new ParcelMonthlyEvapotranspirationDto { ParcelID = parcel.ParcelID, ParcelNumber = parcel.ParcelNumber, EvapotranspirationRate = null, WaterMonth = i, WaterYear = year, IsEmpty = true });
                }
            }

            var parcelLedgerMeasuredUsages = dbContext.ParcelLedgers
                .Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionDate.Year == year && x.TransactionTypeID == TransactionTypeIDMeasuredUsage).Select(x => x.AsDto()).ToList();

            // fill in the real values into the full set
            foreach (var parcelMonthlyEvapotranspirationDto in parcelMonthlyEvapotranspirations)
            {
                var existing = parcelLedgerMeasuredUsages.SingleOrDefault(x =>
                    x.ParcelID == parcelMonthlyEvapotranspirationDto.ParcelID &&
                    x.WaterYear == parcelMonthlyEvapotranspirationDto.WaterYear &&
                    x.TransactionDate.Month == parcelMonthlyEvapotranspirationDto.WaterMonth);
                if (existing != null)
                {
                    parcelMonthlyEvapotranspirationDto.IsEmpty = false;
                    parcelMonthlyEvapotranspirationDto.EvapotranspirationRate = -existing.TransactionAmount;
                }
            }

            var parcelLedgerMeasuredUsageCorrections = dbContext.ParcelLedgers
                .Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionDate.Year == year &&
                            x.TransactionTypeID == TransactionTypeIDMeasuredUsageCorrection).Select(x => x.AsDto())
                .ToList();

            // fill in the real values into the full set
            foreach (var parcelMonthlyEvapotranspirationDto in parcelMonthlyEvapotranspirations)
            {
                var existing = parcelLedgerMeasuredUsageCorrections.SingleOrDefault(x =>
                    x.ParcelID == parcelMonthlyEvapotranspirationDto.ParcelID &&
                    x.WaterYear == parcelMonthlyEvapotranspirationDto.WaterYear &&
                    x.TransactionDate.Month == parcelMonthlyEvapotranspirationDto.WaterMonth);
                if (existing != null)
                {
                    parcelMonthlyEvapotranspirationDto.IsEmpty = false;
                    parcelMonthlyEvapotranspirationDto.OverriddenEvapotranspirationRate = -existing.TransactionAmount;
                }
            }
            return parcelMonthlyEvapotranspirations;
        }
        public static int SaveParcelMonthlyUsageOverrides(RioDbContext dbContext, int accountID, int waterYear,
    List<ParcelMonthlyEvapotranspirationDto> overriddenParcelMonthlyEvapotranspirationDtos)
        {
            var parcelLedgersToSave = overriddenParcelMonthlyEvapotranspirationDtos
                .Where(x => x.OverriddenEvapotranspirationRate != null).Select(x =>
                {
                    var transactionDate = CreateTransactionDateFromYearMonth(x.WaterYear, x.WaterMonth);
                    return new ParcelLedger()
                    {
                        ParcelID = x.ParcelID,
                        TransactionDate = transactionDate,
                        TransactionTypeID = TransactionTypeIDMeasuredUsageCorrection,
                        TransactionAmount = -x.OverriddenEvapotranspirationRate.Value,
                        TransactionDescription =
                            $"A correction to {transactionDate:MMMM yyyy} has been applied to this water account"
                    };
                }).ToList();

            var parcelDtos = Parcel.ListByAccountIDAndYear(dbContext, accountID, waterYear).ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var existingParcelLedgers =
                dbContext.ParcelLedgers.Where(x =>
                    parcelIDs.Contains(x.ParcelID) && x.TransactionDate.Year == waterYear && x.TransactionTypeID == TransactionTypeIDMeasuredUsageCorrection).ToList();

            var allInDatabase = dbContext.ParcelLedgers;

            var countChanging = parcelLedgersToSave.Count(x =>
                                    existingParcelLedgers.Any(y =>
                                    y.ParcelID == x.ParcelID && y.TransactionDate == x.TransactionDate &&
                                    y.TransactionAmount != x.TransactionAmount));

            var countBeingIntroduced = parcelLedgersToSave.Count(x =>
                                            !existingParcelLedgers.Any(y =>
                                            y.ParcelID == x.ParcelID && y.TransactionDate == x.TransactionDate));

            var countBeingRemoved = existingParcelLedgers.Count(x =>
                                        !parcelLedgersToSave.Any(y =>
                                        y.ParcelID == x.ParcelID && y.TransactionDate == x.TransactionDate));

            existingParcelLedgers.Merge(parcelLedgersToSave, allInDatabase,
                (x, y) => x.ParcelID == y.ParcelID && x.TransactionDate == y.TransactionDate,
                (x, y) => x.TransactionAmount = y.TransactionAmount);
            dbContext.SaveChanges();
            return countChanging + countBeingIntroduced + countBeingRemoved;
        }

        private static DateTime CreateTransactionDateFromYearMonth(int waterYear, int waterMonth)
        {
            return new DateTime(waterYear, waterMonth, 2);
        }


        public static List<LandownerAllocationBreakdownDto> GetLandownerAllocationBreakdownForYear(RioDbContext dbContext, int year)
        {
            var accountParcelWaterYearOwnershipsByYear = Entities.Parcel.AccountParcelWaterYearOwnershipsByYear(dbContext, year);

            var parcelAllocations = GetAllocationsImpl(dbContext)
                .Where(x => x.TransactionDate.Year == year);

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
                    parcelOwnershipAndAllocations => parcelOwnershipAndAllocations.ParcelAllocation.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.ParcelOwnership.AccountID,
                        y.TransactionTypeID,
                        y.TransactionAmount
                    })
                .ToList()
                .GroupBy(x => x.AccountID)
                .Select(x => new LandownerAllocationBreakdownDto()
                {
                    AccountID = x.Key,
                    Allocations = new Dictionary<int, decimal>(
                        //unlike above, there may be many ParcelAllocations per Account per Allocation Type, so we need an additional grouping.
                        x.GroupBy(z => z.TransactionTypeID)
                            .Select(y =>
                                new KeyValuePair<int, decimal>(y.Key,
                                    y.Sum(x => x.TransactionAmount))))
                })
                .ToList();
        }

    }
}