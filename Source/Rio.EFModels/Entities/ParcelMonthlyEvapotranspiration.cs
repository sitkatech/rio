using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class ParcelMonthlyEvapotranspiration
    {
        public static List<ParcelMonthlyEvapotranspirationDto> ListByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelMonthlyEvapotranspirations = dbContext.ParcelMonthlyEvapotranspiration
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID);

            return parcelMonthlyEvapotranspirations.Any()
                ? parcelMonthlyEvapotranspirations.Select(x => x.AsDto()).ToList()
                : new List<ParcelMonthlyEvapotranspirationDto>();
        }

        public static List<ParcelMonthlyEvapotranspirationDto> ListByParcelIDAndWaterYear(RioDbContext dbContext, int parcelID, int waterYear)
        {
            var parcelMonthlyEvapotranspirations = dbContext.ParcelMonthlyEvapotranspiration
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID && x.WaterYear == waterYear);

            return parcelMonthlyEvapotranspirations.Any()
                ? parcelMonthlyEvapotranspirations.Select(x => x.AsDto()).ToList()
                : new List<ParcelMonthlyEvapotranspirationDto>();
        }
    }
}