using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLedger
    {
        public static List<ParcelLedgerDto> ListAllocationsByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcelLedgers = dbContext.ParcelLedgers.Include(x => x.TransactionType)
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID && x.TransactionType.IsAllocation);

            return parcelLedgers.Any()
                ? parcelLedgers.Select(x => x.AsDto()).ToList()
                : new List<ParcelLedgerDto>();
        }

        public static List<ParcelLedgerDto> ListAllocationsByParcelID(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcelLedgers = dbContext.ParcelLedgers.Include(x => x.TransactionType)
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID) && x.TransactionType.IsAllocation);

            return parcelLedgers.Any()
                ? parcelLedgers.Select(x => x.AsDto()).ToList()
                : new List<ParcelLedgerDto>();
        }
    }
}