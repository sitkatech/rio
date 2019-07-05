using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocation
    {
        public static List<ParcelAllocationDto> Upsert(RioDbContext dbContext, int parcelID, List<ParcelAllocationUpsertDto> parcelAllocationUpsertDtos)
        {
            foreach (var parcelAllocationUpsertDto in parcelAllocationUpsertDtos)
            {
                var parcelAllocation = dbContext.ParcelAllocation
                    .SingleOrDefault(x => x.ParcelID == parcelID && x.WaterYear == parcelAllocationUpsertDto.WaterYear);

                if (parcelAllocation == null)
                {
                    parcelAllocation = new ParcelAllocation
                    {
                        ParcelID = parcelID,
                        WaterYear = parcelAllocationUpsertDto.WaterYear
                    };
                    dbContext.ParcelAllocation.Add(parcelAllocation);
                }

                parcelAllocation.AcreFeetAllocated = parcelAllocationUpsertDto.AcreFeetAllocated;
            }
            dbContext.SaveChanges();

            return ListByParcelID(dbContext, parcelID);
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