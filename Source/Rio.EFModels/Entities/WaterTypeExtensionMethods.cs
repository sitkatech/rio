using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class WaterTypeExtensionMethods
    {
        public static WaterTypeDto AsDto(this WaterType waterType)
        {
            return new WaterTypeDto()
            {
                WaterTypeID = waterType.WaterTypeID,
                WaterTypeName = waterType.WaterTypeName,
                IsAppliedProportionally =
                    waterType.IsSourcedFromApi ?
                        WaterTypeApplicationTypeEnum.Api :
                        waterType.IsAppliedProportionally ? 
                            WaterTypeApplicationTypeEnum.Proportionally :
                            WaterTypeApplicationTypeEnum.Spreadsheet,
                WaterTypeDefinition = waterType.WaterTypeDefinition,
                SortOrder = waterType.SortOrder
            };

        }
    }
}
