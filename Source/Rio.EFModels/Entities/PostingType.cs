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
            var roles = dbContext.PostingType
                .AsNoTracking()
                .Select(x => x.AsDto());

            return roles;
        }

        public static PostingTypeDto GetByPostingTypeID(RioDbContext dbContext, int postingTypeID)
        {
            var postingType = dbContext.PostingType
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
