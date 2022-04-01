using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class PostingTypes
    {
        public static IEnumerable<PostingTypeDto> List(RioDbContext dbContext)
        {
            return PostingType.All.Select(x => x.AsDto());
        }

        public static PostingTypeDto GetByPostingTypeID(RioDbContext dbContext, int postingTypeID)
        {
            return PostingType.AllLookupDictionary[postingTypeID]?.AsDto();
        }
    }
}
