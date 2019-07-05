using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static class ParcelMonthlyEvapotranspirationExtensionMethods
    {
        public static ParcelMonthlyEvapotranspirationDto AsDto(this ParcelMonthlyEvapotranspiration parcelMonthlyEvapotranspiration)
        {
            return new ParcelMonthlyEvapotranspirationDto()
            {
                WaterYear = parcelMonthlyEvapotranspiration.WaterYear,
                WaterMonth = parcelMonthlyEvapotranspiration.WaterMonth,
                EvapotranspirationRate = parcelMonthlyEvapotranspiration.EvapotranspirationRate
            };
        }
    }
}