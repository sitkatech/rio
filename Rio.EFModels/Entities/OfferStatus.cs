using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class OfferStatuses
    {
        public static IEnumerable<OfferStatusDto> List(RioDbContext dbContext)
        {
            return OfferStatus.All.Select(x => x.AsDto());
        }

        public static OfferStatusDto GetByOfferStatusID(RioDbContext dbContext, int offerStatusID)
        {
            return OfferStatus.AllLookupDictionary[offerStatusID]?.AsDto();
        }
    }
}