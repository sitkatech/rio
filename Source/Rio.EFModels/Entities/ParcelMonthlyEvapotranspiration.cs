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
            var parcelMonthlyEvapotranspirations = dbContext.ParcelMonthlyEvapotranspirations.Include(x => x.Parcel)
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID);

            return parcelMonthlyEvapotranspirations.Any()
                ? parcelMonthlyEvapotranspirations.Select(x => x.AsDto()).ToList()
                : new List<ParcelMonthlyEvapotranspirationDto>();
        }

        public static List<ParcelMonthlyEvapotranspirationDto> ListByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelMonthlyEvapotranspirations = dbContext.ParcelMonthlyEvapotranspirations.Include(x => x.Parcel)
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
                    parcelMonthlyEvapotranspirations.Add(new ParcelMonthlyEvapotranspirationDto {ParcelID = parcel.ParcelID, ParcelNumber = parcel.ParcelNumber, EvapotranspirationRate = null, WaterMonth = i, WaterYear = year, IsEmpty = true});
                }
            }

            var parcelMonthlyEvapotranspirationsFromDB = dbContext.ParcelMonthlyEvapotranspirations.Include(x => x.Parcel)
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
                    parcelMonthlyEvapotranspirationDto.IsEmpty = existing.EvapotranspirationRate == null;
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
            var postedDtos = overriddenParcelMonthlyEvapotranspirationDtos.Where(x => !x.IsEmpty || (x.IsEmpty && x.OverriddenEvapotranspirationRate != null)).Select(x => new ParcelMonthlyEvapotranspiration()
            {
                ParcelID = x.ParcelID,
                WaterYear = x.WaterYear,
                WaterMonth = x.WaterMonth,
                //Don't write to EvapotranspirationRate. Only OpenET imports should adjust that data
                //EvapotranspirationRate = x.EvapotranspirationRate,
                OverriddenEvapotranspirationRate = x.OverriddenEvapotranspirationRate
            }).ToList();

            var parcelDtos = Parcel.ListByAccountIDAndYear(dbContext, accountID, waterYear).ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var existingParcelMonthlyEvapotranspirationDtos =
                dbContext.ParcelMonthlyEvapotranspirations.Where(x =>
                    parcelIDs.Contains(x.ParcelID) && x.WaterYear == waterYear).ToList();

            var allInDatabase = dbContext.ParcelMonthlyEvapotranspirations;

            var countChanging = postedDtos.Count(x =>
                                    existingParcelMonthlyEvapotranspirationDtos.Any(y =>
                                    y.ParcelID == x.ParcelID && y.WaterYear == x.WaterYear && y.WaterMonth == x.WaterMonth &&
                                    y.OverriddenEvapotranspirationRate != x.OverriddenEvapotranspirationRate));

            var countBeingIntroduced = postedDtos.Count(x => 
                                            !existingParcelMonthlyEvapotranspirationDtos.Any(y =>
                                            y.ParcelID == x.ParcelID && y.WaterYear == x.WaterYear && y.WaterMonth == x.WaterMonth));

            var countBeingRemoved = existingParcelMonthlyEvapotranspirationDtos.Count(x =>
                                        !postedDtos.Any(y =>
                                        y.ParcelID == x.ParcelID && y.WaterYear == x.WaterYear && y.WaterMonth == x.WaterMonth));

            existingParcelMonthlyEvapotranspirationDtos.Merge(postedDtos, allInDatabase,
                (x, y) => x.ParcelID == y.ParcelID && x.WaterMonth == y.WaterMonth && x.WaterYear == y.WaterYear,
                (x, y) => x.OverriddenEvapotranspirationRate = y.OverriddenEvapotranspirationRate);
            dbContext.SaveChanges();
            return countChanging + countBeingIntroduced + countBeingRemoved;
        }
    }
}