using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static class ParcelMonthlyEvapotranspirationExtensionMethods
    {
        public static ParcelMonthlyEvapotranspirationDto AsDto(this ParcelMonthlyEvapotranspiration parcelMonthlyEvapotranspiration)
        {
            return new ParcelMonthlyEvapotranspirationDto()
            {
                ParcelID = parcelMonthlyEvapotranspiration.ParcelID,
                ParcelNumber = parcelMonthlyEvapotranspiration.Parcel.ParcelNumber,
                WaterYear = parcelMonthlyEvapotranspiration.WaterYear,
                WaterMonth = parcelMonthlyEvapotranspiration.WaterMonth,
                EvapotranspirationRate = parcelMonthlyEvapotranspiration.EvapotranspirationRate,
                OverriddenEvapotranspirationRate = parcelMonthlyEvapotranspiration.OverriddenEvapotranspirationRate
            };
        }
    }
}