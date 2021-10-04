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
        public static int BulkSetAllocation(RioDbContext dbContext, ParcelAllocationUpsertDto parcelAllocationUpsertDto)
        {
            // delete existing parcel allocations
            var existingParcelAllocations = dbContext.ParcelAllocations.Where(x => x.WaterYear == parcelAllocationUpsertDto.WaterYear && x.ParcelAllocationTypeID == parcelAllocationUpsertDto.ParcelAllocationTypeID);
            if (existingParcelAllocations.Any())
            {
                dbContext.ParcelAllocations.RemoveRange(existingParcelAllocations);
                dbContext.SaveChanges();
            }

            var parcels = dbContext.Parcels.AsNoTracking().OrderBy(x => x.ParcelID).ToList();
            foreach (var parcel in parcels)
            {
                var parcelAllocation = new ParcelAllocation
                {
                    ParcelID = parcel.ParcelID,
                    WaterYear = parcelAllocationUpsertDto.WaterYear,
                    ParcelAllocationTypeID = parcelAllocationUpsertDto.ParcelAllocationTypeID,
                    AcreFeetAllocated = parcelAllocationUpsertDto.AcreFeetAllocated * (decimal)parcel.ParcelAreaInAcres
                };
                dbContext.ParcelAllocations.Add(parcelAllocation);
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
            var existingParcelAllocations = dbContext.ParcelAllocations.Where(x =>
                x.WaterYear == waterYear && x.ParcelAllocationTypeID == parcelAllocationType);
            if (existingParcelAllocations.Any())
            {
                dbContext.ParcelAllocations.RemoveRange(existingParcelAllocations);
                dbContext.SaveChanges();
            }

            var parcelAllocations = new List<ParcelAllocation>();
            foreach (var record in records)
            {
                var parcel = dbContext.Parcels.First(x => x.ParcelNumber == record.APN);
                parcelAllocations.Add(new ParcelAllocation()
                {
                    ParcelID = parcel.ParcelID,
                    AcreFeetAllocated =
                        (decimal)(record.AllocationQuantity * parcel.ParcelAreaInAcres),
                    WaterYear = waterYear,
                    ParcelAllocationTypeID = parcelAllocationType
                });
            }

            dbContext.ParcelAllocations.AddRange(parcelAllocations);
            dbContext.SaveChanges();
        }
    }
}