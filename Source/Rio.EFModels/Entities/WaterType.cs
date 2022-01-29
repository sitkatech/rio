using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class WaterType
    {
        public static List<WaterTypeDto> GetWaterTypes(RioDbContext dbContext)
        {
            return dbContext.WaterTypes.AsNoTracking().OrderBy(x=>x.SortOrder).Select(x => x.AsDto()).ToList();
        }
    }
}