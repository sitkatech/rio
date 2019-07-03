using Rio.Models.DataTransferObjects.Posting;

namespace Rio.EFModels.Entities
{
    public static class PostingTypeExtensionMethods
    {
        public static PostingTypeDto AsDto(this PostingType postingType)
        {
            return new PostingTypeDto()
            {
                PostingTypeID = postingType.PostingTypeID,
                PostingTypeName = postingType.PostingTypeName,
                PostingTypeDisplayName = postingType.PostingTypeDisplayName
            };
        }
    }
}