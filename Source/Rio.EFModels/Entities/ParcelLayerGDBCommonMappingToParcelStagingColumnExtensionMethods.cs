using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class ParcelLayerGDBCommonMappingToParcelStagingColumnExtensionMethods
    {
        public static ParcelLayerGDBCommonMappingToParcelStagingColumnDto AsDto(
            this ParcelLayerGDBCommonMappingToParcelStagingColumn parcelLayerGDBCommonMappingToParcelStagingColumn)
        {
            return new ParcelLayerGDBCommonMappingToParcelStagingColumnDto()
            {
                ParcelLayerGDBCommonMappingToParcelColumnID = parcelLayerGDBCommonMappingToParcelStagingColumn
                    .ParcelLayerGDBCommonMappingToParcelColumnID,
                ParcelNumber = parcelLayerGDBCommonMappingToParcelStagingColumn.ParcelNumber,
                OwnerName = parcelLayerGDBCommonMappingToParcelStagingColumn.OwnerName
            };
        }
    }
}