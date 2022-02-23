//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Offer]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class OfferExtensionMethods
    {
        public static OfferDto AsDto(this Offer offer)
        {
            var offerDto = new OfferDto()
            {
                OfferID = offer.OfferID,
                Trade = offer.Trade.AsDto(),
                OfferDate = offer.OfferDate,
                Quantity = offer.Quantity,
                Price = offer.Price,
                OfferStatus = offer.OfferStatus.AsDto(),
                OfferNotes = offer.OfferNotes,
                CreateAccount = offer.CreateAccount.AsDto()
            };
            DoCustomMappings(offer, offerDto);
            return offerDto;
        }

        static partial void DoCustomMappings(Offer offer, OfferDto offerDto);

        public static OfferSimpleDto AsSimpleDto(this Offer offer)
        {
            var offerSimpleDto = new OfferSimpleDto()
            {
                OfferID = offer.OfferID,
                TradeID = offer.TradeID,
                OfferDate = offer.OfferDate,
                Quantity = offer.Quantity,
                Price = offer.Price,
                OfferStatusID = offer.OfferStatusID,
                OfferNotes = offer.OfferNotes,
                CreateAccountID = offer.CreateAccountID
            };
            DoCustomSimpleDtoMappings(offer, offerSimpleDto);
            return offerSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Offer offer, OfferSimpleDto offerSimpleDto);
    }
}