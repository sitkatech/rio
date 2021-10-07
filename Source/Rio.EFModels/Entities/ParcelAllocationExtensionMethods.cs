using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public static class ParcelAllocationExtensionMethods
    {
        public static ParcelAllocationDto AsDto(this ParcelAllocation parcelAllocation)
        {
            return new ParcelAllocationDto()
            {
                WaterTypeID = parcelAllocation.WaterTypeID,
                ParcelID = parcelAllocation.ParcelID,
                WaterYear = parcelAllocation.WaterYear,
                AcreFeetAllocated = parcelAllocation.AcreFeetAllocated,
                ParcelAllocationID = parcelAllocation.ParcelAllocationID
            };
        }
    }
}