//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferType]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferTypeExtensionMethods
    {
        public static WaterTransferTypeDto AsDto(this WaterTransferType waterTransferType)
        {
            var waterTransferTypeDto = new WaterTransferTypeDto()
            {
                WaterTransferTypeID = waterTransferType.WaterTransferTypeID,
                WaterTransferTypeName = waterTransferType.WaterTransferTypeName,
                WaterTransferTypeDisplayName = waterTransferType.WaterTransferTypeDisplayName
            };
            DoCustomMappings(waterTransferType, waterTransferTypeDto);
            return waterTransferTypeDto;
        }

        static partial void DoCustomMappings(WaterTransferType waterTransferType, WaterTransferTypeDto waterTransferTypeDto);

        public static WaterTransferTypeSimpleDto AsSimpleDto(this WaterTransferType waterTransferType)
        {
            var waterTransferTypeSimpleDto = new WaterTransferTypeSimpleDto()
            {
                WaterTransferTypeID = waterTransferType.WaterTransferTypeID,
                WaterTransferTypeName = waterTransferType.WaterTransferTypeName,
                WaterTransferTypeDisplayName = waterTransferType.WaterTransferTypeDisplayName
            };
            DoCustomSimpleDtoMappings(waterTransferType, waterTransferTypeSimpleDto);
            return waterTransferTypeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterTransferType waterTransferType, WaterTransferTypeSimpleDto waterTransferTypeSimpleDto);
    }
}