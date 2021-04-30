using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using Rio.Models.DataTransferObjects.WaterUsage;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocation
    {
        public static List<ParcelAllocationDto> Upsert(RioDbContext dbContext, int parcelID, List<ParcelAllocationUpsertDto> parcelAllocationUpsertDtos)
        {
            // delete existing parcel allocations
            var existingParcelAllocations = dbContext.ParcelAllocation.Where(x => x.ParcelID == parcelID);
            if (existingParcelAllocations.Any())
            {
                dbContext.ParcelAllocation.RemoveRange(existingParcelAllocations);
                dbContext.SaveChanges();
            }

            foreach (var parcelAllocationUpsertDto in parcelAllocationUpsertDtos)
            {
                var parcelAllocation = dbContext.ParcelAllocation
                    .SingleOrDefault(x => x.ParcelID == parcelID && x.WaterYear == parcelAllocationUpsertDto.WaterYear && x.ParcelAllocationTypeID == parcelAllocationUpsertDto.ParcelAllocationTypeID);

                if (parcelAllocation == null)
                {
                    parcelAllocation = new ParcelAllocation
                    {
                        ParcelID = parcelID,
                        WaterYear = parcelAllocationUpsertDto.WaterYear,
                        ParcelAllocationTypeID = parcelAllocationUpsertDto.ParcelAllocationTypeID
                    };
                    dbContext.ParcelAllocation.Add(parcelAllocation);
                }

                parcelAllocation.AcreFeetAllocated = parcelAllocationUpsertDto.AcreFeetAllocated;
            }
            dbContext.SaveChanges();

            return ListByParcelID(dbContext, parcelID);
        }
        public static int BulkSetAllocation(RioDbContext dbContext, ParcelAllocationUpsertDto parcelAllocationUpsertDto)
        {
            // delete existing parcel allocations
            var existingParcelAllocations = dbContext.ParcelAllocation.Where(x => x.WaterYear == parcelAllocationUpsertDto.WaterYear && x.ParcelAllocationTypeID == parcelAllocationUpsertDto.ParcelAllocationTypeID);
            if (existingParcelAllocations.Any())
            {
                dbContext.ParcelAllocation.RemoveRange(existingParcelAllocations);
                dbContext.SaveChanges();
            }

            var parcels = dbContext.Parcel.AsNoTracking().OrderBy(x => x.ParcelID).ToList();
            foreach (var parcel in parcels)
            {
                var parcelAllocation = new ParcelAllocation
                {
                    ParcelID = parcel.ParcelID,
                    WaterYear = parcelAllocationUpsertDto.WaterYear,
                    ParcelAllocationTypeID = parcelAllocationUpsertDto.ParcelAllocationTypeID,
                    AcreFeetAllocated = parcelAllocationUpsertDto.AcreFeetAllocated * (decimal)parcel.ParcelAreaInAcres
                };
                dbContext.ParcelAllocation.Add(parcelAllocation);
            }
            dbContext.SaveChanges();
            return parcels.Count;
        }

        //Keep as reference for setting Allocation proportionally across an account and by volume
        //public static void BulkSetAllocation(RioDbContext dbContext, List<BulkSetAllocationCSV> records, int waterYear, int parcelAllocationType)
        //{
        //    // delete existing parcel allocations
        //    var existingParcelAllocations = dbContext.ParcelAllocation.Where(x =>
        //        x.WaterYear == waterYear && x.ParcelAllocationTypeID == parcelAllocationType);
        //    if (existingParcelAllocations.Any())
        //    {
        //        dbContext.ParcelAllocation.RemoveRange(existingParcelAllocations);
        //        dbContext.SaveChanges();
        //    }

        //    // select parcels owned by accounts from upload and group by accounts to associate allocation volumes with list of parcels
        //    var accountAllocationVolumes = Parcel.AccountParcelWaterYearOwnershipsByYear(dbContext, waterYear).ToList().GroupBy(x => x.Account.AccountNumber)
        //        .Where(x => records.Select(y => y.AccountNumber).Contains(x.Key)).Join(records,
        //            account => account.Key, record => record.AccountNumber,
        //            (x, y) => new { Parcels = x.Select(z => z.Parcel).ToList(), y.AllocationVolume });


        //    var parcelAllocations = new List<ParcelAllocation>();
        //    // apportion the reconciliation volumes to their lists of parcels by area percentage
        //    foreach (var record in accountAllocationVolumes)
        //    {
        //        var parcels = record.Parcels;
        //        var sum = parcels.Sum(x => x.ParcelAreaInAcres);
        //        parcelAllocations.AddRange(parcels.Select(x => new ParcelAllocation()
        //        {
        //            ParcelID = x.ParcelID,
        //            AcreFeetAllocated =
        //                (decimal)(record.AllocationVolume * (x.ParcelAreaInAcres / sum)),
        //            WaterYear = waterYear,
        //            ParcelAllocationTypeID = parcelAllocationType
        //        }));
        //    }

        //    dbContext.ParcelAllocation.AddRange(parcelAllocations);
        //    dbContext.SaveChanges();
        //}

        public static void BulkSetAllocation(RioDbContext dbContext, List<BulkSetAllocationCSV> records, int waterYear,
            int parcelAllocationType)
        {
            //delete existing parcel allocations
            var existingParcelAllocations = dbContext.ParcelAllocation.Where(x =>
                x.WaterYear == waterYear && x.ParcelAllocationTypeID == parcelAllocationType);
            if (existingParcelAllocations.Any())
            {
                dbContext.ParcelAllocation.RemoveRange(existingParcelAllocations);
                dbContext.SaveChanges();
            }

            var parcelAllocations = new List<ParcelAllocation>();
            foreach (var record in records)
            {
                var parcel = dbContext.Parcel.First(x => x.ParcelNumber == record.APN);
                parcelAllocations.Add(new ParcelAllocation()
                {
                    ParcelID = parcel.ParcelID,
                    AcreFeetAllocated =
                        (decimal)(record.AllocationQuantity * parcel.ParcelAreaInAcres),
                    WaterYear = waterYear,
                    ParcelAllocationTypeID = parcelAllocationType
                });
            }

            dbContext.ParcelAllocation.AddRange(parcelAllocations);
            dbContext.SaveChanges();
        }

        public static List<ParcelAllocationDto> ListByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelAllocations = dbContext.ParcelAllocation
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID);

            return parcelAllocations.Any()
                ? parcelAllocations.Select(x => x.AsDto()).ToList()
                : new List<ParcelAllocationDto>();
        }

        public static List<ParcelAllocationDto> ListByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelAllocations = dbContext.ParcelAllocation
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID));

            return parcelAllocations.Any()
                ? parcelAllocations.Select(x => x.AsDto()).ToList()
                : new List<ParcelAllocationDto>();
        }

        public static List<ParcelAllocationBreakdownDto> GetParcelAllocationBreakdownForYear(RioDbContext dbContext, int year)
        {
            return dbContext.ParcelAllocation.AsNoTracking()
                .Where(x => x.WaterYear == year)
                .ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => new ParcelAllocationBreakdownDto
                {
                    ParcelID = x.Key,
                    // There's at most one ParcelAllocation per Parcel per AllocationType, so we just need to read elements of the group into this dictionary
                    Allocations = new Dictionary<int, decimal>(x.Select(y =>
                        new KeyValuePair<int, decimal>(y.ParcelAllocationTypeID,
                            y.AcreFeetAllocated)))
                }).ToList();
        }

        public static List<LandownerAllocationBreakdownDto> GetLandownerAllocationBreakdownForYear(RioDbContext dbContext, int year)
        {
            var accountParcelWaterYearOwnershipsByYear = Entities.Parcel.AccountParcelWaterYearOwnershipsByYear(dbContext, year);

            var parcelAllocations = dbContext.ParcelAllocation.AsNoTracking().Where(x => x.WaterYear == year);

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
                        y.ParcelAllocationTypeID,
                        y.AcreFeetAllocated
                    })
                .ToList()
                .GroupBy(x => x.AccountID)
                .Select(x => new LandownerAllocationBreakdownDto()
                {
                    AccountID = x.Key,
                    Allocations = new Dictionary<int, decimal>(
                        //unlike above, there may be many ParcelAllocations per Account per Allocation Type, so we need an additional grouping.
                        x.GroupBy(z => z.ParcelAllocationTypeID)
                        .Select(y =>
                        new KeyValuePair<int, decimal>(y.Key,
                            y.Sum(x => x.AcreFeetAllocated))))
                })
                .ToList();
        }

    }
}