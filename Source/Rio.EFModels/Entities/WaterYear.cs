using System;
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
            return dbContext.WaterYear.OrderByDescending(x => x.Year).Select(x => x.AsDto()).ToList();
        }

        public static WaterYearDto GetDefaultYearToDisplay(RioDbContext dbContext)
        {
            var year = DateTime.Now.Year;
            return dbContext.WaterYear.Any(x => x.Year == year) ? 
                dbContext.WaterYear.Single(x => x.Year == year).AsDto() : 
                dbContext.WaterYear.OrderByDescending(x => x.Year).First().AsDto();
        }

        public static WaterYearDto GetByWaterYearID(RioDbContext dbContext, int waterYearID)
        {
            return dbContext.WaterYear.SingleOrDefault(x => x.WaterYearID == waterYearID).AsDto();
        }

        public static WaterYearDto Finalize(RioDbContext dbContext, int waterYearID)
        {
            var waterYear = dbContext.WaterYear.Single(x => x.WaterYearID == waterYearID);

            waterYear.FinalizeDate = DateTime.UtcNow;

            dbContext.SaveChanges();
            dbContext.Entry(waterYear).Reload();
            return GetByWaterYearID(dbContext, waterYearID);
        }

        public static List<WaterYearDto> ListNonFinalized(RioDbContext dbContext)
        {
            return dbContext.WaterYear
                .Where(x => x.FinalizeDate == null)
                .OrderByDescending(x => x.Year)
                .Select(x => x.AsDto())
                .ToList();
        }

        public static WaterYearDto GetByYear(RioDbContext dbContext, int waterYear)
        {
            return dbContext.WaterYear.SingleOrDefault(x => x.Year == waterYear).AsDto();
        }

        public static List<WaterYearDto> ListBetweenYears(RioDbContext dbContext, int startYear, int endYear)
        {
            return dbContext.WaterYear.Where(x => x.Year >= startYear && x.Year <= endYear).OrderByDescending(x => x.Year).Select(x => x.AsDto()).ToList();
        }

        public static void UpdateParcelLayerUpdateDateForID(RioDbContext dbContext, int waterYearId)
        {
            var waterYear = dbContext.WaterYear.Single(x => x.WaterYearID == waterYearId);

            waterYear.ParcelLayerUpdateDate = DateTime.UtcNow;

            dbContext.SaveChanges();
        }
    }
}
