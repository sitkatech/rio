using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class WaterYearMonth
    {
        public static IQueryable<WaterYearMonth> GetWaterYearMonthImpl(RioDbContext dbContext)
        {
            return dbContext.WaterYearMonth
                .Include(x => x.WaterYear)
                .AsNoTracking();
        }
        public static List<WaterYearMonthDto> List(RioDbContext dbContext)
        {
            return GetWaterYearMonthImpl(dbContext).OrderByDescending(x => x.WaterYear.Year).ThenByDescending(x => x.Month).Select(x => x.AsDto()).ToList();
        }

        public static WaterYearMonthDto GetByWaterYearMonthID(RioDbContext dbContext, int waterYearMonthID)
        {
            return GetWaterYearMonthImpl(dbContext).SingleOrDefault(x => x.WaterYearMonthID == waterYearMonthID).AsDto();
        }

        public static WaterYearMonthDto Finalize(RioDbContext dbContext, int waterYearMonthID)
        {
            var waterYear = dbContext.WaterYearMonth.Single(x => x.WaterYearMonthID == waterYearMonthID);

            waterYear.FinalizeDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(waterYear).Reload();
            return GetByWaterYearMonthID(dbContext, waterYearMonthID);
        }

        public static List<WaterYearMonthDto> ListNonFinalized(RioDbContext dbContext)
        {
            return GetWaterYearMonthImpl(dbContext)
                .Where(x => x.FinalizeDate == null)
                .OrderByDescending(x => x.WaterYear.Year).ThenByDescending(x => x.Month)
                .Select(x => x.AsDto())
                .ToList();
        }

        public static WaterYearMonthDto GetByYearAndMonth(RioDbContext dbContext, int waterYear, int waterMonth)
        {
            return GetWaterYearMonthImpl(dbContext).SingleOrDefault(x => x.WaterYear.Year == waterYear && x.Month == waterMonth).AsDto();
        }
    }
}