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

        public static List<ParcelMonthlyEvapotranspirationDto> ListByParcelIDAndYear(RioDbContext dbContext, List<int> parcelIDs,
            List<ParcelDto> parcels, int year)
        {
            var parcelMonthlyEvapotranspirations = new List<ParcelMonthlyEvapotranspirationDto>();
            // make the full matrix of months * parcels and populate with zero/empty
            foreach (var parcel in parcels)
            {
                for (var i = 1; i < 13; i++)
                {
                    parcelMonthlyEvapotranspirations.Add(new ParcelMonthlyEvapotranspirationDto {ParcelID = parcel.ParcelID, ParcelNumber = parcel.ParcelNumber, EvapotranspirationRate = 0, WaterMonth = i, WaterYear = year, IsEmpty = true});
                }
            }

            var parcelMonthlyEvapotranspirationsFromDB = dbContext.ParcelMonthlyEvapotranspiration.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.WaterYear == year).Select(x => x.AsDto()).ToList();

            // fill in the real values into the full set
            foreach (var parcelMonthlyEvapotranspirationDto in parcelMonthlyEvapotranspirations)
            {
                var existing = parcelMonthlyEvapotranspirationsFromDB.SingleOrDefault(x =>
                    x.ParcelID == parcelMonthlyEvapotranspirationDto.ParcelID &&
                    x.WaterYear == parcelMonthlyEvapotranspirationDto.WaterYear &&
                    x.WaterMonth == parcelMonthlyEvapotranspirationDto.WaterMonth);
                if (existing != null)
                {
                    parcelMonthlyEvapotranspirationDto.IsEmpty = false;
                    parcelMonthlyEvapotranspirationDto.EvapotranspirationRate = existing.EvapotranspirationRate;
                    parcelMonthlyEvapotranspirationDto.OverriddenEvapotranspirationRate =
                        existing.OverriddenEvapotranspirationRate;

                }
            }
            return parcelMonthlyEvapotranspirations;
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