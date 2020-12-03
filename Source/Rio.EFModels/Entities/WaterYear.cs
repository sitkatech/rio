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
            return dbContext.WaterYear.Single(x => x.Year == DateTime.Now.Year).AsDto();
        }
    }
}
