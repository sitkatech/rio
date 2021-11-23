using Rio.Models.DataTransferObjects;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class CustomRichText
    {
        public static CustomRichTextDto GetByCustomRichTextTypeID(RioDbContext dbContext, int customRichTextTypeID)
        {
            var customRichText = dbContext.CustomRichTexts
                .Include(x => x.CustomRichTextType)
                .SingleOrDefault(x => x.CustomRichTextTypeID == customRichTextTypeID);

            return customRichText?.AsDto();
        }

        public static CustomRichTextDto UpdateCustomRichText(RioDbContext dbContext, int customRichTextTypeID,
            CustomRichTextDto customRichTextUpdateDto)
        {
            var customRichText = dbContext.CustomRichTexts
                .Include(x => x.CustomRichTextType)
                .SingleOrDefault(x => x.CustomRichTextTypeID == customRichTextTypeID);

            // null check occurs in calling endpoint method.
            customRichText.CustomRichTextContent = customRichTextUpdateDto.CustomRichTextContent;

            dbContext.SaveChanges();

            return customRichText.AsDto();
        }
    }
}
