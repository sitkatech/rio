﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class WaterYear
    {
        public static List<WaterYearDto> List(RioDbContext dbContext)
        {
            return dbContext.WaterYears.OrderByDescending(x => x.Year).Select(x => x.AsDto()).ToList();
        }

        public static WaterYearDto GetDefaultYearToDisplay(RioDbContext dbContext)
        {
            var year = DateTime.Now.Year;
            return dbContext.WaterYears.Any(x => x.Year == year) ? 
                dbContext.WaterYears.Single(x => x.Year == year).AsDto() : 
                dbContext.WaterYears.OrderByDescending(x => x.Year).First().AsDto();
        }

        public static WaterYearDto GetByWaterYearID(RioDbContext dbContext, int waterYearID)
        {
            return dbContext.WaterYears.SingleOrDefault(x => x.WaterYearID == waterYearID).AsDto();
        }

        public static List<WaterYearDto> ListBetweenYears(RioDbContext dbContext, int startYear, int endYear)
        {
            return dbContext.WaterYears.Where(x => x.Year >= startYear && x.Year <= endYear).OrderByDescending(x => x.Year).Select(x => x.AsDto()).ToList();
        }

        public static void UpdateParcelLayerUpdateDateForID(RioDbContext dbContext, int waterYearId)
        {
            var waterYear = dbContext.WaterYears.Single(x => x.WaterYearID == waterYearId);

            waterYear.ParcelLayerUpdateDate = DateTime.UtcNow;

            dbContext.SaveChanges();
        }

        public static List<WaterYearDto> GetSubsequentWaterYearsInclusive(RioDbContext dbContext, int effectiveWaterYearId)
        {
            var waterYear = GetByWaterYearID(dbContext, effectiveWaterYearId);

            if (waterYear == null)
            {
                return null;
            }

            return dbContext.WaterYears
                .Where(x => x.Year >= waterYear.Year)
                .OrderByDescending(x => x.Year)
                .Select(x => x.AsDto())
                .ToList();
        }
    }
}
