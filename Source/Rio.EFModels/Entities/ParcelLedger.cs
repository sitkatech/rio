using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLedger
    {

        private static IQueryable<ParcelLedger> GetParcelLedgersImpl(RioDbContext dbContext)
        {
            return dbContext.ParcelLedgers.Include(x => x.TransactionType)
                .Include(x => x.WaterType)
                .Include(x => x.Parcel)
                .AsNoTracking();
        }

        private static IQueryable<ParcelLedger> GetAllocationsImpl(RioDbContext dbContext)
        {
            return GetParcelLedgersImpl(dbContext)
                .Where(x => x.TransactionTypeID == (int) TransactionTypeEnum.Allocation);
        }


        public static List<ParcelLedgerDto> ListLedgerEntriesByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelLedgers = GetParcelLedgersImpl(dbContext)
                .Where(x => parcelID == x.ParcelID)
                .Select(x => x.AsDto())
                .ToList();

            return parcelLedgers;
        }

        public static List<ParcelAllocationBreakdownDto> GetParcelAllocationBreakdownForYear(RioDbContext dbContext, int year)
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

        public static IQueryable<ParcelLedger> GetUsagesByParcelIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            var usageTransactionTypeIDs = new List<int> { 17, 18, 19 };
            return GetByTransactionTypeIDsAndParcelIDs(dbContext, parcelIDs, usageTransactionTypeIDs);
        }

        private static IQueryable<ParcelLedger> GetByTransactionTypeIDAndParcelIDs(RioDbContext dbContext, List<int> parcelIDs, int transactionTypeID)
        {
            return GetParcelLedgersImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionTypeID == transactionTypeID);
        }

        private static IQueryable<ParcelLedger> GetByTransactionTypeIDsAndParcelIDs(RioDbContext dbContext, List<int> parcelIDs, List<int> transactionTypeIDs)
        {
            return GetParcelLedgersImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID) && transactionTypeIDs.Contains(x.TransactionTypeID));
        }

        private static DateTime CreateEffectiveDateFromYearMonth(int waterYear, int waterMonth)
        {
            return new DateTime(waterYear, waterMonth, 2);
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
            var parcelIDs = Entities.Parcel.ListByAccountID(dbContext, accountID)
                .Select(x => x.ParcelID);

            var parcelLedgerDtos = GetParcelLedgersImpl(dbContext)
                .Include(x => x.Parcel)
                .Where(x => parcelIDs.Contains(x.ParcelID))
                .Select(x => new ParcelLedgerDto()
                {
                    ParcelLedgerID = x.ParcelLedgerID,
                    ParcelID = x.ParcelID,
                    ParcelNumber = x.Parcel.ParcelNumber,
                    TransactionDate = x.TransactionDate,
                    EffectiveDate = x.EffectiveDate,
                    TransactionType = x.TransactionType.AsDto(),
                    WaterType = (x.WaterType != null ? x.WaterType.AsDto() : null),
                    TransactionAmount = x.TransactionAmount,
                    TransactionDescription = x.TransactionDescription
                })
                .OrderByDescending(x => x.EffectiveDate)
                .ToList();

            return parcelLedgerDtos;
        }

        public static ParcelLedgerDto getByParcelLedgerID(RioDbContext dbContext, int parcelLedgerID)
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

            return getByParcelLedgerID(dbContext, parcelLedger.ParcelLedgerID);
        }
    }
}