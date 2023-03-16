//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterYear]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterYearExtensionMethods
    {
        public static WaterYearDto AsDto(this WaterYear waterYear)
        {
            var waterYearDto = new WaterYearDto()
            {
                WaterYearID = waterYear.WaterYearID,
                Year = waterYear.Year,
                ParcelLayerUpdateDate = waterYear.ParcelLayerUpdateDate,
                OverconsumptionRate = waterYear.OverconsumptionRate
            };
            DoCustomMappings(waterYear, waterYearDto);
            return waterYearDto;
        }

        static partial void DoCustomMappings(WaterYear waterYear, WaterYearDto waterYearDto);

        public static WaterYearSimpleDto AsSimpleDto(this WaterYear waterYear)
        {
            var waterYearSimpleDto = new WaterYearSimpleDto()
            {
                WaterYearID = waterYear.WaterYearID,
                Year = waterYear.Year,
                ParcelLayerUpdateDate = waterYear.ParcelLayerUpdateDate,
                OverconsumptionRate = waterYear.OverconsumptionRate
            };
            DoCustomSimpleDtoMappings(waterYear, waterYearSimpleDto);
            return waterYearSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterYear waterYear, WaterYearSimpleDto waterYearSimpleDto);
    }
}