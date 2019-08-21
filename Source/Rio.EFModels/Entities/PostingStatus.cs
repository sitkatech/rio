using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Posting;

namespace Rio.EFModels.Entities
{
    public partial class PostingStatus
    {
        public static IEnumerable<PostingStatusDto> List(RioDbContext dbContext)
        {
            var postingStatusDtos = dbContext.PostingStatus
                .AsNoTracking()
                .Select(x => x.AsDto());

            return postingStatusDtos;
        }

        public static PostingStatusDto GetByPostingStatusID(RioDbContext dbContext, int postingStatusID)
        {
            var postingStatus = dbContext.PostingStatus
                .AsNoTracking()
                .SingleOrDefault(x => x.PostingStatusID == postingStatusID);

            return postingStatus?.AsDto();
        }
    }

    public enum PostingStatusEnum
    {
        Open = 1,
        Closed = 2
    }
}