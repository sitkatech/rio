using Rio.Models.DataTransferObjects.Posting;

namespace Rio.EFModels.Entities
{
    public static class PostingStatusExtensionMethods
    {
        public static PostingStatusDto AsDto(this PostingStatus postingStatus)
        {
            return new PostingStatusDto()
            {
                PostingStatusID = postingStatus.PostingStatusID,
                PostingStatusName = postingStatus.PostingStatusName,
                PostingStatusDisplayName = postingStatus.PostingStatusDisplayName
            };
        }
    }
}