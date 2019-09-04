using Rio.Models.DataTransferObjects.Posting;

namespace Rio.EFModels.Entities
{
    public static class PostingExtensionMethods
    {
        public static PostingDto AsDto(this Posting posting)
        {
            return new PostingDto()
            {
                PostingID = posting.PostingID,
                PostingDate = posting.PostingDate,
                PostingDescription = posting.PostingDescription,
                Quantity = posting.Quantity,
                AvailableQuantity = posting.AvailableQuantity,
                Price = posting.Price,
                CreateUser = posting.CreateUser.AsSimpleDto(),
                PostingType = posting.PostingType.AsDto(),
                PostingStatus = posting.PostingStatus.AsDto()
            };
        }
    }
}