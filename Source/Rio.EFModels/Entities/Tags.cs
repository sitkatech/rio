using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class Tags
    {
        public static List<TagDto> ListAsDto(RioDbContext dbContext)
        {
            var tags = dbContext.Tags.Select(x => x.AsDto()).ToList();
            return tags;
        }

        public static List<TagDto> ListByParcelIDAsDto(RioDbContext dbContext, int parcelID)
        {
            var parcelTags = dbContext.ParcelTags
                .Include(x => x.Tag)
                .Where(x => x.ParcelID == parcelID);

            var tagDtos = parcelTags.Select(x => x.Tag.AsDto()).ToList();
            return tagDtos;
        }
    }
}