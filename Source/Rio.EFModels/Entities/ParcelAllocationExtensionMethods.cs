using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public static class ParcelAllocationExtensionMethods
    {
        public static ParcelAllocationDto AsDto(this ParcelAllocation parcelAllocation)
        {
            return new ParcelAllocationDto()
            {
                ParcelAllocationTypeID = parcelAllocation.ParcelAllocationTypeID,
                ParcelID = parcelAllocation.ParcelID,
                WaterYear = parcelAllocation.WaterYear,
                AcreFeetAllocated = parcelAllocation.AcreFeetAllocated
            };
        }
    }
}