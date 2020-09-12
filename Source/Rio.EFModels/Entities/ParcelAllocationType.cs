using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocationType
    {
        public static List<ParcelAllocationTypeDto> GetParcelAllocationTypes(RioDbContext dbContext)
        {
            return dbContext.ParcelAllocationType.AsNoTracking().OrderBy(x=>x.ParcelAllocationTypeID).Select(x => x.AsDto()).ToList();
        }
    }
}