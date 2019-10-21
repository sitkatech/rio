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
            //// delete existing parcels registered
            //var existingWaterTransferRegistrationParcels = dbContext.ParcelAllocation.Where(x => x.ParcelAllocationTypeID == waterTransferRegistration.WaterTransferRegistrationID);
            //if (existingWaterTransferRegistrationParcels.Any())
            //{
            //    dbContext.WaterTransferRegistrationParcel.RemoveRange(existingWaterTransferRegistrationParcels);
            //    dbContext.SaveChanges();
            //}


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