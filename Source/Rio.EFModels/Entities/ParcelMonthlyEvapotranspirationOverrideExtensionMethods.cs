using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static class ParcelMonthlyEvapotranspirationOverrideExtensionMethods
    {
        public static ParcelMonthlyEvapotranspirationOverrideDto AsDto(this ParcelMonthlyEvapotranspirationOverride parcelMonthlyEvapotranspirationOverride)
        {
            return new ParcelMonthlyEvapotranspirationOverrideDto()
            {
                ParcelID = parcelMonthlyEvapotranspirationOverride.ParcelID,
                ParcelNumber = parcelMonthlyEvapotranspirationOverride.Parcel.ParcelNumber,
                WaterYear = parcelMonthlyEvapotranspirationOverride.WaterYear,
                WaterMonth = parcelMonthlyEvapotranspirationOverride.WaterMonth,
                OverriddenEvapotranspirationRate = parcelMonthlyEvapotranspirationOverride.OverriddenEvapotranspirationRate
            };
        }
    }
}
