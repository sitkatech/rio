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
            var tagDtos = dbContext.Tags.Select(x => x.AsDto()).ToList();

            var taggedParcelCountByTagID = dbContext.ParcelTags.GroupBy(x => x.TagID)
                .Select(x => new {TagID = x.Key, TaggedParcelCount = x.Count()});

            foreach (var tagDto in tagDtos)
            {
                tagDto.TaggedParcelsCount = taggedParcelCountByTagID.SingleOrDefault(x => x.TagID == tagDto.TagID)?.TaggedParcelCount;
            }
            return tagDtos;
        }

        public static TagDto GetByIDAsDto(RioDbContext dbContext, int tagID)
        {
            var tag = dbContext.Tags.SingleOrDefault(x => x.TagID == tagID);
            return tag?.AsDto();
        }

        public static List<TagDto> ListByParcelIDAsDto(RioDbContext dbContext, int parcelID)
        {
            var tagDtos = dbContext.ParcelTags
                .Include(x => x.Tag)
                .Where(x => x.ParcelID == parcelID)
                .OrderBy(x => x.Tag.TagName)
                .Select(x => x.Tag.AsDto())
                .ToList();

            return tagDtos;
        }

        public static void Delete(RioDbContext dbContext, Tag tag)
        {
            var parcelTagsToRemove = dbContext.ParcelTags.Where(x => x.TagID == tag.TagID);
            dbContext.ParcelTags.RemoveRange(parcelTagsToRemove);

            dbContext.Tags.Remove(tag);
            dbContext.SaveChanges();
        }
    }
}