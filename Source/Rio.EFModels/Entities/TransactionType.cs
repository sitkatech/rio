using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class TransactionType
    {
        public static List<TransactionTypeDto> GetTransactionTypes(RioDbContext dbContext)
        {
            return dbContext.TransactionTypes.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => x.AsDto()).ToList();
        }
        public static List<TransactionTypeDto> GetAllocationTypes(RioDbContext dbContext)
        {
            return dbContext.TransactionTypes.Where(x => x.IsAllocation == true).AsNoTracking().OrderBy(x => x.SortOrder).Select(x => x.AsDto()).ToList();
        }
    }
}