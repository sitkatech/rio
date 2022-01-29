//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Posting]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class PostingExtensionMethods
    {
        public static PostingDto AsDto(this Posting posting)
        {
            var postingDto = new PostingDto()
            {
                PostingID = posting.PostingID,
                PostingType = posting.PostingType.AsDto(),
                PostingDate = posting.PostingDate,
                CreateAccount = posting.CreateAccount.AsDto(),
                Quantity = posting.Quantity,
                Price = posting.Price,
                PostingDescription = posting.PostingDescription,
                PostingStatus = posting.PostingStatus.AsDto(),
                AvailableQuantity = posting.AvailableQuantity,
                CreateUser = posting.CreateUser?.AsDto()
            };
            DoCustomMappings(posting, postingDto);
            return postingDto;
        }

        static partial void DoCustomMappings(Posting posting, PostingDto postingDto);

        public static PostingSimpleDto AsSimpleDto(this Posting posting)
        {
            var postingSimpleDto = new PostingSimpleDto()
            {
                PostingID = posting.PostingID,
                PostingTypeID = posting.PostingTypeID,
                PostingDate = posting.PostingDate,
                CreateAccountID = posting.CreateAccountID,
                Quantity = posting.Quantity,
                Price = posting.Price,
                PostingDescription = posting.PostingDescription,
                PostingStatusID = posting.PostingStatusID,
                AvailableQuantity = posting.AvailableQuantity,
                CreateUserID = posting.CreateUserID
            };
            DoCustomSimpleDtoMappings(posting, postingSimpleDto);
            return postingSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Posting posting, PostingSimpleDto postingSimpleDto);
    }
}