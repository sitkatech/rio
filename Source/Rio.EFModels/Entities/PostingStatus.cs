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
            var postingStatusDtos = dbContext.PostingStatuses
                .AsNoTracking()
                .Select(x => x.AsDto());

            return postingStatusDtos;
        }

        public static PostingStatusDto GetByPostingStatusID(RioDbContext dbContext, int postingStatusID)
        {
            var postingStatus = dbContext.PostingStatuses
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