//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelLayerGDBCommonMappingToParcelStagingColumn]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelLayerGDBCommonMappingToParcelStagingColumnExtensionMethods
    {
        public static ParcelLayerGDBCommonMappingToParcelStagingColumnDto AsDto(this ParcelLayerGDBCommonMappingToParcelStagingColumn parcelLayerGDBCommonMappingToParcelStagingColumn)
        {
            var parcelLayerGDBCommonMappingToParcelStagingColumnDto = new ParcelLayerGDBCommonMappingToParcelStagingColumnDto()
            {
                ParcelLayerGDBCommonMappingToParcelColumnID = parcelLayerGDBCommonMappingToParcelStagingColumn.ParcelLayerGDBCommonMappingToParcelColumnID,
                ParcelNumber = parcelLayerGDBCommonMappingToParcelStagingColumn.ParcelNumber,
                OwnerName = parcelLayerGDBCommonMappingToParcelStagingColumn.OwnerName
            };
            DoCustomMappings(parcelLayerGDBCommonMappingToParcelStagingColumn, parcelLayerGDBCommonMappingToParcelStagingColumnDto);
            return parcelLayerGDBCommonMappingToParcelStagingColumnDto;
        }

        static partial void DoCustomMappings(ParcelLayerGDBCommonMappingToParcelStagingColumn parcelLayerGDBCommonMappingToParcelStagingColumn, ParcelLayerGDBCommonMappingToParcelStagingColumnDto parcelLayerGDBCommonMappingToParcelStagingColumnDto);

        public static ParcelLayerGDBCommonMappingToParcelStagingColumnSimpleDto AsSimpleDto(this ParcelLayerGDBCommonMappingToParcelStagingColumn parcelLayerGDBCommonMappingToParcelStagingColumn)
        {
            var parcelLayerGDBCommonMappingToParcelStagingColumnSimpleDto = new ParcelLayerGDBCommonMappingToParcelStagingColumnSimpleDto()
            {
                ParcelLayerGDBCommonMappingToParcelColumnID = parcelLayerGDBCommonMappingToParcelStagingColumn.ParcelLayerGDBCommonMappingToParcelColumnID,
                ParcelNumber = parcelLayerGDBCommonMappingToParcelStagingColumn.ParcelNumber,
                OwnerName = parcelLayerGDBCommonMappingToParcelStagingColumn.OwnerName
            };
            DoCustomSimpleDtoMappings(parcelLayerGDBCommonMappingToParcelStagingColumn, parcelLayerGDBCommonMappingToParcelStagingColumnSimpleDto);
            return parcelLayerGDBCommonMappingToParcelStagingColumnSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelLayerGDBCommonMappingToParcelStagingColumn parcelLayerGDBCommonMappingToParcelStagingColumn, ParcelLayerGDBCommonMappingToParcelStagingColumnSimpleDto parcelLayerGDBCommonMappingToParcelStagingColumnSimpleDto);
    }
}