using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class WaterYearMonthExtensionMethods
    {
        public static WaterYearMonthDto AsDto(this WaterYearMonth waterYearMonth)
        {
            return new WaterYearMonthDto()
            {
                WaterYearMonthID = waterYearMonth.WaterYearMonthID,
                WaterYear = waterYearMonth.WaterYear.AsDto(),
                Month = waterYearMonth.Month,
                FinalizeDate = waterYearMonth.FinalizeDate
            };
        }
    }
}