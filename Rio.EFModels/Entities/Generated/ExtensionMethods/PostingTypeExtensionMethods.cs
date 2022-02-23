//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PostingType]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class PostingTypeExtensionMethods
    {
        public static PostingTypeDto AsDto(this PostingType postingType)
        {
            var postingTypeDto = new PostingTypeDto()
            {
                PostingTypeID = postingType.PostingTypeID,
                PostingTypeName = postingType.PostingTypeName,
                PostingTypeDisplayName = postingType.PostingTypeDisplayName
            };
            DoCustomMappings(postingType, postingTypeDto);
            return postingTypeDto;
        }

        static partial void DoCustomMappings(PostingType postingType, PostingTypeDto postingTypeDto);

        public static PostingTypeSimpleDto AsSimpleDto(this PostingType postingType)
        {
            var postingTypeSimpleDto = new PostingTypeSimpleDto()
            {
                PostingTypeID = postingType.PostingTypeID,
                PostingTypeName = postingType.PostingTypeName,
                PostingTypeDisplayName = postingType.PostingTypeDisplayName
            };
            DoCustomSimpleDtoMappings(postingType, postingTypeSimpleDto);
            return postingTypeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(PostingType postingType, PostingTypeSimpleDto postingTypeSimpleDto);
    }
}