using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocation
    {
        public static ParcelAllocationDto Upsert(RioDbContext dbContext, int parcelID, int waterYear, ParcelAllocationUpsertDto parcelAllocationUpsertDto)
        {
            var parcelAllocation = dbContext.ParcelAllocation
                .SingleOrDefault(x => x.ParcelID == parcelID && x.WaterYear == waterYear);

            if (parcelAllocation == null)
            {
                parcelAllocation = new ParcelAllocation
                {
                    ParcelID = parcelID,
                    WaterYear = waterYear
                };
                dbContext.ParcelAllocation.Add(parcelAllocation);
            }

            parcelAllocation.AcreFeetAllocated = parcelAllocationUpsertDto.AcreFeetAllocated;
            dbContext.SaveChanges();
            dbContext.Entry(parcelAllocation).Reload();

            return GetByParcelIDAndWaterYear(dbContext, parcelAllocation.ParcelID, parcelAllocation.WaterYear);
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

        public static ParcelAllocationDto GetByParcelIDAndWaterYear(RioDbContext dbContext, int parcelID, int waterYear)
        {
            var parcelAllocation = dbContext.ParcelAllocation
                .AsNoTracking()
                .SingleOrDefault(x => x.ParcelID == parcelID && x.WaterYear == waterYear);

            var parcelAllocationDto = parcelAllocation?.AsDto();
            return parcelAllocationDto;
        }

        public static void Delete(RioDbContext dbContext, int parcelAllocationID)
        {
            var parcelAllocation = dbContext.ParcelAllocation
                .Single(x => x.ParcelAllocationID == parcelAllocationID);
            dbContext.ParcelAllocation.Remove(parcelAllocation);
        }
    }
}