using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class ParcelMonthlyEvapotranspirationOverride
    {
        public static List<ParcelMonthlyEvapotranspirationOverrideDto> ListByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelMonthlyEvapotranspirationOverrides = dbContext.ParcelMonthlyEvapotranspirationOverride.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID);

            return parcelMonthlyEvapotranspirationOverrides.Any()
                ? parcelMonthlyEvapotranspirationOverrides.Select(x => x.AsDto()).ToList()
                : new List<ParcelMonthlyEvapotranspirationOverrideDto>();
        }

        public static List<ParcelMonthlyEvapotranspirationOverrideDto> ListByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelMonthlyEvapotranspirationOverrides = dbContext.ParcelMonthlyEvapotranspirationOverride.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID));

            return parcelMonthlyEvapotranspirationOverrides.Any()
                ? parcelMonthlyEvapotranspirationOverrides.Select(x => x.AsDto()).ToList()
                : new List<ParcelMonthlyEvapotranspirationOverrideDto>();
        }
    }
}