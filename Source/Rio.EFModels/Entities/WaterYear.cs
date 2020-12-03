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
            return dbContext.WaterYear.OrderByDescending(x => x.Year).First().AsDto();
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
    }
}
