using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLedger
    {
        public static List<ParcelLedgerDto> ListAllocationsByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelLedgers = dbContext.ParcelLedgers.Include(x => x.TransactionType)
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID && x.TransactionType.IsAllocation);

            return parcelLedgers.Any()
                ? parcelLedgers.Select(x => x.AsDto()).ToList()
                : new List<ParcelLedgerDto>();
        }

        public static List<ParcelLedgerDto> ListAllocationsByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelLedgers = dbContext.ParcelLedgers.Include(x => x.TransactionType)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionType.IsAllocation);

            return parcelLedgers.Any()
                ? parcelLedgers.Select(x => x.AsDto()).ToList()
                : new List<ParcelLedgerDto>();
        }

        public static List<ParcelAllocationBreakdownDto> GetParcelAllocationBreakdownForYear(RioDbContext dbContext, int year)
        {
            var parcelAllocationBreakdownForYear = dbContext.ParcelLedgers.Include(x => x.TransactionType).AsNoTracking()
                .Where(x => x.TransactionDate.Year == year &&
                            x.TransactionType.IsAllocation == true)
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
        public static List<LandownerAllocationBreakdownDto> GetLandownerAllocationBreakdownForYear(RioDbContext dbContext, int year)
        {
            var accountParcelWaterYearOwnershipsByYear = Entities.Parcel.AccountParcelWaterYearOwnershipsByYear(dbContext, year);

            var parcelAllocations = dbContext.ParcelLedgers.Include(x => x.TransactionType).AsNoTracking()
                .Where(x => x.TransactionDate.Year == year &&
                            x.TransactionType.IsAllocation == true);

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