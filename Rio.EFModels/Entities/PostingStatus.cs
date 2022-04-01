using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class PostingStatuses
    {
        public static IEnumerable<PostingStatusDto> List(RioDbContext dbContext)
        {
            return PostingStatus.All.Select(x => x.AsDto());
        }

        public static PostingStatusDto GetByPostingStatusID(RioDbContext dbContext, int postingStatusID)
        {
            return PostingStatus.AllLookupDictionary[postingStatusID]?.AsDto();
        }
    }
}