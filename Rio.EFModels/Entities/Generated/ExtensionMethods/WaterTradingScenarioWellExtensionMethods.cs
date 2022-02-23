//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTradingScenarioWell]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTradingScenarioWellExtensionMethods
    {
        public static WaterTradingScenarioWellDto AsDto(this WaterTradingScenarioWell waterTradingScenarioWell)
        {
            var waterTradingScenarioWellDto = new WaterTradingScenarioWellDto()
            {
                WaterTradingScenarioWellID = waterTradingScenarioWell.WaterTradingScenarioWellID,
                WaterTradingScenarioWellCountyName = waterTradingScenarioWell.WaterTradingScenarioWellCountyName
            };
            DoCustomMappings(waterTradingScenarioWell, waterTradingScenarioWellDto);
            return waterTradingScenarioWellDto;
        }

        static partial void DoCustomMappings(WaterTradingScenarioWell waterTradingScenarioWell, WaterTradingScenarioWellDto waterTradingScenarioWellDto);

        public static WaterTradingScenarioWellSimpleDto AsSimpleDto(this WaterTradingScenarioWell waterTradingScenarioWell)
        {
            var waterTradingScenarioWellSimpleDto = new WaterTradingScenarioWellSimpleDto()
            {
                WaterTradingScenarioWellID = waterTradingScenarioWell.WaterTradingScenarioWellID,
                WaterTradingScenarioWellCountyName = waterTradingScenarioWell.WaterTradingScenarioWellCountyName
            };
            DoCustomSimpleDtoMappings(waterTradingScenarioWell, waterTradingScenarioWellSimpleDto);
            return waterTradingScenarioWellSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterTradingScenarioWell waterTradingScenarioWell, WaterTradingScenarioWellSimpleDto waterTradingScenarioWellSimpleDto);
    }
}