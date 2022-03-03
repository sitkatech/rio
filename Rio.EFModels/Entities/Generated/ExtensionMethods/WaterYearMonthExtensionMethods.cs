//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterYearMonth]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterYearMonthExtensionMethods
    {
        public static WaterYearMonthDto AsDto(this WaterYearMonth waterYearMonth)
        {
            var waterYearMonthDto = new WaterYearMonthDto()
            {
                WaterYearMonthID = waterYearMonth.WaterYearMonthID,
                WaterYear = waterYearMonth.WaterYear.AsDto(),
                Month = waterYearMonth.Month,
                FinalizeDate = waterYearMonth.FinalizeDate
            };
            DoCustomMappings(waterYearMonth, waterYearMonthDto);
            return waterYearMonthDto;
        }

        static partial void DoCustomMappings(WaterYearMonth waterYearMonth, WaterYearMonthDto waterYearMonthDto);

        public static WaterYearMonthSimpleDto AsSimpleDto(this WaterYearMonth waterYearMonth)
        {
            var waterYearMonthSimpleDto = new WaterYearMonthSimpleDto()
            {
                WaterYearMonthID = waterYearMonth.WaterYearMonthID,
                WaterYearID = waterYearMonth.WaterYearID,
                Month = waterYearMonth.Month,
                FinalizeDate = waterYearMonth.FinalizeDate
            };
            DoCustomSimpleDtoMappings(waterYearMonth, waterYearMonthSimpleDto);
            return waterYearMonthSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterYearMonth waterYearMonth, WaterYearMonthSimpleDto waterYearMonthSimpleDto);
    }
}