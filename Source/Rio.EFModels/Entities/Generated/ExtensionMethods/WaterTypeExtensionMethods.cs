//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterType]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTypeExtensionMethods
    {
        public static WaterTypeDto AsDto(this WaterType waterType)
        {
            var waterTypeDto = new WaterTypeDto()
            {
                WaterTypeID = waterType.WaterTypeID,
                WaterTypeName = waterType.WaterTypeName,
                IsAppliedProportionally = waterType.IsAppliedProportionally,
                WaterTypeDefinition = waterType.WaterTypeDefinition,
                IsSourcedFromApi = waterType.IsSourcedFromApi,
                SortOrder = waterType.SortOrder,
                IsUserDefined = waterType.IsUserDefined
            };
            DoCustomMappings(waterType, waterTypeDto);
            return waterTypeDto;
        }

        static partial void DoCustomMappings(WaterType waterType, WaterTypeDto waterTypeDto);

        public static WaterTypeSimpleDto AsSimpleDto(this WaterType waterType)
        {
            var waterTypeSimpleDto = new WaterTypeSimpleDto()
            {
                WaterTypeID = waterType.WaterTypeID,
                WaterTypeName = waterType.WaterTypeName,
                IsAppliedProportionally = waterType.IsAppliedProportionally,
                WaterTypeDefinition = waterType.WaterTypeDefinition,
                IsSourcedFromApi = waterType.IsSourcedFromApi,
                SortOrder = waterType.SortOrder,
                IsUserDefined = waterType.IsUserDefined
            };
            DoCustomSimpleDtoMappings(waterType, waterTypeSimpleDto);
            return waterTypeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterType waterType, WaterTypeSimpleDto waterTypeSimpleDto);
    }
}