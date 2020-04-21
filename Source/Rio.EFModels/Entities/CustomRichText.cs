using Rio.Models.DataTransferObjects;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class CustomRichText
    {


        public static CustomRichTextDto GetByCustomRichTextTypeID(RioDbContext dbContext, int customRichTextTypeID)
        {
            var customRichText = dbContext.CustomRichText
                .SingleOrDefault(x => x.CustomRichTextID == customRichTextTypeID);

            return customRichText?.AsDto();
        }
    }
}
