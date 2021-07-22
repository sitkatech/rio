using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Posting;

namespace Rio.EFModels.Entities
{
    public partial class PostingType
    {
        public static IEnumerable<PostingTypeDto> List(RioDbContext dbContext)
        {
            var roles = dbContext.PostingTypes
                .AsNoTracking()
                .Select(x => x.AsDto());

            return roles;
        }

        public static PostingTypeDto GetByPostingTypeID(RioDbContext dbContext, int postingTypeID)
        {
            var postingType = dbContext.PostingTypes
                .AsNoTracking()
                .SingleOrDefault(x => x.PostingTypeID == postingTypeID);

            return postingType?.AsDto();
        }
    }

    public enum PostingTypeEnum
    {
        OfferToBuy = 1,
        OfferToSell = 2
    }
}
