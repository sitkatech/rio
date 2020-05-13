using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;
using Rio.Models.DataTransferObjects.ParcelAllocation;

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
                    AcreFeetAllocated = parcelAllocationUpsertDto.AcreFeetAllocated * (decimal) parcel.ParcelAreaInAcres
                };
                dbContext.ParcelAllocation.Add(parcelAllocation);
            }
            dbContext.SaveChanges();
            return parcels.Count;
        }

        public static void BulkSetAllocation(RioDbContext dbContext, List<BulkSetAllocationCSV> records, int waterYear, int parcelAllocationType)
        {
            // delete existing parcel allocations
            var existingParcelAllocations = dbContext.ParcelAllocation.Where(x =>
                x.WaterYear == waterYear && x.ParcelAllocationTypeID == parcelAllocationType);
            if (existingParcelAllocations.Any())
            {
                dbContext.ParcelAllocation.RemoveRange(existingParcelAllocations);
                dbContext.SaveChanges();
            }

            // select parcels owned by accounts from upload and group by accounts to associate allocation volumes with list of parcels
            var accountAllocationVolumes = Parcel.vParcelOwnershipsByYear(dbContext, waterYear).ToList().GroupBy(x => x.Account.AccountNumber)
                .Where(x => records.Select(y => y.AccountNumber).Contains(x.Key)).Join(records,
                    account => account.Key, record => record.AccountNumber,
                    (x, y) => new { Parcels = x.Select(z => z.Parcel).ToList(), y.AllocationVolume });


            var parcelAllocations = new List<ParcelAllocation>();
            // apportion the reconciliation volumes to their lists of parcels by area percentage
            foreach (var record in accountAllocationVolumes)
            {
                var parcels = record.Parcels;
                var sum = parcels.Sum(x => x.ParcelAreaInAcres);
                parcelAllocations.AddRange(parcels.Select(x => new ParcelAllocation()
                {
                    ParcelID = x.ParcelID,
                    AcreFeetAllocated =
                        (decimal)(record.AllocationVolume * (x.ParcelAreaInAcres / sum)),
                    WaterYear = waterYear,
                    ParcelAllocationTypeID = parcelAllocationType
                }));
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
    }
}