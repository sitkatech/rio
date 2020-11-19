using System;
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
                EvapotranspirationRate = parcelMonthlyEvapotranspiration.EvapotranspirationRate.HasValue ? Math.Round(parcelMonthlyEvapotranspiration.EvapotranspirationRate.Value, 1) : (decimal?)null,
                OverriddenEvapotranspirationRate = parcelMonthlyEvapotranspiration.OverriddenEvapotranspirationRate.HasValue ? Math.Round(parcelMonthlyEvapotranspiration.OverriddenEvapotranspirationRate.Value, 1) : (decimal?)null,
                IsEmpty = parcelMonthlyEvapotranspiration.EvapotranspirationRate == null
            };
        }
    }
}