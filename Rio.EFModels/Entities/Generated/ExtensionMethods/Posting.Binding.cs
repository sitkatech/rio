//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Posting]
namespace Rio.EFModels.Entities
{
    public partial class Posting
    {
        public PostingType PostingType => PostingType.AllLookupDictionary[PostingTypeID];
        public PostingStatus PostingStatus => PostingStatus.AllLookupDictionary[PostingStatusID];
    }
}