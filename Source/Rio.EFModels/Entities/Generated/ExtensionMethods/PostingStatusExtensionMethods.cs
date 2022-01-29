//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PostingStatus]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class PostingStatusExtensionMethods
    {
        public static PostingStatusDto AsDto(this PostingStatus postingStatus)
        {
            var postingStatusDto = new PostingStatusDto()
            {
                PostingStatusID = postingStatus.PostingStatusID,
                PostingStatusName = postingStatus.PostingStatusName,
                PostingStatusDisplayName = postingStatus.PostingStatusDisplayName
            };
            DoCustomMappings(postingStatus, postingStatusDto);
            return postingStatusDto;
        }

        static partial void DoCustomMappings(PostingStatus postingStatus, PostingStatusDto postingStatusDto);

        public static PostingStatusSimpleDto AsSimpleDto(this PostingStatus postingStatus)
        {
            var postingStatusSimpleDto = new PostingStatusSimpleDto()
            {
                PostingStatusID = postingStatus.PostingStatusID,
                PostingStatusName = postingStatus.PostingStatusName,
                PostingStatusDisplayName = postingStatus.PostingStatusDisplayName
            };
            DoCustomSimpleDtoMappings(postingStatus, postingStatusSimpleDto);
            return postingStatusSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(PostingStatus postingStatus, PostingStatusSimpleDto postingStatusSimpleDto);
    }
}