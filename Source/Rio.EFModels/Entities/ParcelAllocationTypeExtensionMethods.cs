using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class ParcelAllocationTypeExtensionMethods
    {
        public static ParcelAllocationTypeDto AsDto(this ParcelAllocationType parcelAllocationType)
        {
            return new ParcelAllocationTypeDto()
            {
                ParcelAllocationTypeID = parcelAllocationType.ParcelAllocationTypeID,
                ParcelAllocationTypeName = parcelAllocationType.ParcelAllocationTypeName,
                IsAppliedProportionally =
                    parcelAllocationType.IsSourcedFromApi ?
                        ParcelAllocationTypeApplicationTypeEnum.Api :
                        parcelAllocationType.IsAppliedProportionally ? 
                            ParcelAllocationTypeApplicationTypeEnum.Proportionally :
                            ParcelAllocationTypeApplicationTypeEnum.Spreadsheet,
                ParcelAllocationTypeDefinition = parcelAllocationType.ParcelAllocationTypeDefinition,
                SortOrder = parcelAllocationType.SortOrder
            };

        }
    }
}
