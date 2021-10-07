using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLedger
    {
        private const int TransactionTypeIDMeasuredUsageCorrection = 18;
        private const int TransactionTypeIDMeasuredUsage = 17;
        public const int TransactionTypeAllocation = 11;

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
                .Where(x => x.TransactionTypeID == TransactionTypeAllocation);
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
                .Where(x => x.TransactionDate.Year == year && x.WaterTypeID != null)
                .ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => new ParcelAllocationBreakdownDto
                {
                    ParcelID = x.Key,
                    // There's at most one ParcelAllocation per Parcel per WaterType, so we just need to read elements of the group into this dictionary
                    Allocations = new Dictionary<int, decimal>(x.Select(y =>
                        new KeyValuePair<int, decimal>(y.WaterTypeID.Value,
                            y.TransactionAmount)))
                }).ToList();
            return parcelAllocationBreakdownForYear;
        }
        public static List<ParcelMonthlyEvapotranspirationDto> ListMonthlyEvapotranspirationsByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelLedgerMeasuredUsages = GetMeasuredUsagesByParcelIDs(dbContext, parcelIDs);

            var parcelMonthlyEvapotranspirationDtos = parcelLedgerMeasuredUsages.Select(x => 
                new ParcelMonthlyEvapotranspirationDto()
                {
                    ParcelID = x.ParcelID,
                    WaterYear = x.TransactionDate.Year,
                    WaterMonth = x.TransactionDate.Month,
                    EvapotranspirationRate = -x.TransactionAmount
                }
            ).ToList();
            
            var parcelLedgerMeasuredUsageCorrections = GetMeasuredUsageCorrectionsByParcelIDs(dbContext, parcelIDs);

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

        private static IQueryable<ParcelLedger> GetMeasuredUsageCorrectionsByParcelIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            return GetByTransactionTypeIDAndParcelIDs(dbContext, parcelIDs, TransactionTypeIDMeasuredUsageCorrection);
        }

        private static IQueryable<ParcelLedger> GetMeasuredUsagesByParcelIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            return GetByTransactionTypeIDAndParcelIDs(dbContext, parcelIDs, TransactionTypeIDMeasuredUsage);
        }

        private static IQueryable<ParcelLedger> GetByTransactionTypeIDAndParcelIDs(RioDbContext dbContext, List<int> parcelIDs,
            int transactionTypeID)
        {
            return dbContext.ParcelLedgers.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionTypeID == transactionTypeID);
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

            var parcelLedgerMeasuredUsages = GetMeasuredUsagesByParcelIDs(dbContext, parcelIDs)
                .Where(x => x.TransactionDate.Year == year).Select(x => x.AsDto()).ToList();

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

            var parcelLedgerMeasuredUsageCorrections = GetMeasuredUsageCorrectionsByParcelIDs(dbContext, parcelIDs)
                .Where(x => x.TransactionDate.Year == year).Select(x => x.AsDto())
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
                    parcelMonthlyEvapotranspirationDto.OverriddenEvapotranspirationRate = -(existing.TransactionAmount + -parcelMonthlyEvapotranspirationDto.EvapotranspirationRate);
                }
            }
            return parcelMonthlyEvapotranspirations;
        }

        public static int SaveParcelMonthlyUsageOverrides(RioDbContext dbContext, int accountID, int waterYear,
            List<ParcelMonthlyEvapotranspirationDto> overriddenParcelMonthlyEvapotranspirationDtos)
        {
            var parcelDtos = Parcel.ListByAccountIDAndYear(dbContext, accountID, waterYear).ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var existingParcelLedgers =
                dbContext.ParcelLedgers
                    .AsNoTracking()
                    .Where(x => x.TransactionDate.Year == waterYear &&
                                parcelIDs.Contains(x.ParcelID) &&
                                (x.TransactionTypeID == TransactionTypeIDMeasuredUsageCorrection ||
                                 x.TransactionTypeID == TransactionTypeIDMeasuredUsage))
                    .ToList();

            var parcelLedgersToSave = new List<ParcelLedger>();
            foreach (var x in overriddenParcelMonthlyEvapotranspirationDtos.Where(x =>
                x.OverriddenEvapotranspirationRate != null))
            {
                var currentMonthlyValue = existingParcelLedgers.Where(y => y.ParcelID == x.ParcelID
                                                                           && y.TransactionDate.Month == x.WaterMonth
                                                                           && y.TransactionDate.Year == x.WaterYear
                ).Sum(y => y.TransactionAmount);
                var transactionAmount = -(x.OverriddenEvapotranspirationRate.Value + currentMonthlyValue);
                if (transactionAmount != 0)
                {
                    var transactionDate = CreateTransactionDateFromYearMonth(x.WaterYear, x.WaterMonth);
                    var parcelLedger = new ParcelLedger
                    {
                        ParcelID = x.ParcelID,
                        TransactionDate = transactionDate,
                        TransactionTypeID = TransactionTypeIDMeasuredUsageCorrection,
                        TransactionAmount = transactionAmount,
                        TransactionDescription =
                            $"A correction to {transactionDate:MMMM yyyy} has been applied to this water account"
                    };
                    parcelLedgersToSave.Add(parcelLedger);
                }
            }

            var parcelLedgersWithCorrections = parcelLedgersToSave.Where(x => x != null).ToList();
            if (parcelLedgersWithCorrections.Any())
            {
                dbContext.ParcelLedgers.AddRange(parcelLedgersWithCorrections);
            }

            dbContext.SaveChanges();
            return parcelLedgersWithCorrections.Count;
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

    }
}