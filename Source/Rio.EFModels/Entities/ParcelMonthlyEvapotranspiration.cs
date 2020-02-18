using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Rio.API.Util;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class ParcelMonthlyEvapotranspiration
    {
        public static List<ParcelMonthlyEvapotranspirationDto> ListByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelMonthlyEvapotranspirations = dbContext.ParcelMonthlyEvapotranspiration.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID);

            return parcelMonthlyEvapotranspirations.Any()
                ? parcelMonthlyEvapotranspirations.Select(x => x.AsDto()).ToList()
                : new List<ParcelMonthlyEvapotranspirationDto>();
        }

        public static List<ParcelMonthlyEvapotranspirationDto> ListByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelMonthlyEvapotranspirations = dbContext.ParcelMonthlyEvapotranspiration.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID));

            return parcelMonthlyEvapotranspirations.Any()
                ? parcelMonthlyEvapotranspirations.Select(x => x.AsDto()).ToList()
                : new List<ParcelMonthlyEvapotranspirationDto>();
        }

        public static int SaveParcelMonthlyUsageOverrides(RioDbContext dbContext, int accountID, int waterYear,
            List<ParcelMonthlyEvapotranspirationDto> overriddenParcelMonthlyEvapotranspirationDtos)
        {
            var postedDtos = overriddenParcelMonthlyEvapotranspirationDtos.Select(x => new ParcelMonthlyEvapotranspiration()
            {
                ParcelID = x.ParcelID,
                WaterYear = x.WaterYear,
                WaterMonth = x.WaterMonth,
                EvapotranspirationRate = x.EvapotranspirationRate,
                OverriddenEvapotranspirationRate = x.OverriddenEvapotranspirationRate
            }).ToList();

            var parcelDtos = Parcel.ListByAccountID(dbContext, accountID, waterYear).ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var existingParcelMonthlyEvapotranspirationDtos =
                dbContext.ParcelMonthlyEvapotranspiration.Where(x =>
                    parcelIDs.Contains(x.ParcelID) && x.WaterYear == waterYear).ToList();

            var allInDatabase = dbContext.ParcelMonthlyEvapotranspiration;

            var countChanging = postedDtos.Count(x => existingParcelMonthlyEvapotranspirationDtos.Any(y =>
                y.ParcelID == x.ParcelID && y.WaterYear == x.WaterYear && y.WaterMonth == x.WaterMonth && (y.OverriddenEvapotranspirationRate != x.OverriddenEvapotranspirationRate || x.OverriddenEvapotranspirationRate != null)));

            existingParcelMonthlyEvapotranspirationDtos.Merge(postedDtos, allInDatabase,
                (x, y) => x.ParcelID == y.ParcelID && x.WaterMonth == y.WaterMonth && x.WaterYear == y.WaterYear,
                (x, y) => x.OverriddenEvapotranspirationRate = y.OverriddenEvapotranspirationRate);
            dbContext.SaveChanges();
            return countChanging;
        }
    }
}