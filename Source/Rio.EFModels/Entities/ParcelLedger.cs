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

            var parcelMonthlyEvapotranspirationsFromDB = dbContext.ParcelLedgers
                .Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionDate.Year == year && x.TransactionTypeID == 17).Select(x => x.AsDto()).ToList();

            // fill in the real values into the full set
            foreach (var parcelMonthlyEvapotranspirationDto in parcelMonthlyEvapotranspirations)
            {
                var existing = parcelMonthlyEvapotranspirationsFromDB.SingleOrDefault(x =>
                    x.ParcelID == parcelMonthlyEvapotranspirationDto.ParcelID &&
                    x.WaterYear == parcelMonthlyEvapotranspirationDto.WaterYear &&
                    x.TransactionDate.Month == parcelMonthlyEvapotranspirationDto.WaterMonth);
                if (existing != null)
                {
                    parcelMonthlyEvapotranspirationDto.IsEmpty = false;
                    parcelMonthlyEvapotranspirationDto.EvapotranspirationRate = -existing.TransactionAmount;
                    parcelMonthlyEvapotranspirationDto.OverriddenEvapotranspirationRate = null;
                    // TODO: pulling override numbers via transaction type ID
                }
            }
            return parcelMonthlyEvapotranspirations;
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