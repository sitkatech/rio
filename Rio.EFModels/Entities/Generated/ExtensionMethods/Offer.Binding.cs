//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Offer]
namespace Rio.EFModels.Entities
{
    public partial class Offer
    {
        public OfferStatus OfferStatus => OfferStatus.AllLookupDictionary[OfferStatusID];
    }
}