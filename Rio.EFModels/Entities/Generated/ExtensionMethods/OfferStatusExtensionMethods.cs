//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OfferStatus]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class OfferStatusExtensionMethods
    {
        public static OfferStatusDto AsDto(this OfferStatus offerStatus)
        {
            var offerStatusDto = new OfferStatusDto()
            {
                OfferStatusID = offerStatus.OfferStatusID,
                OfferStatusName = offerStatus.OfferStatusName,
                OfferStatusDisplayName = offerStatus.OfferStatusDisplayName
            };
            DoCustomMappings(offerStatus, offerStatusDto);
            return offerStatusDto;
        }

        static partial void DoCustomMappings(OfferStatus offerStatus, OfferStatusDto offerStatusDto);

        public static OfferStatusSimpleDto AsSimpleDto(this OfferStatus offerStatus)
        {
            var offerStatusSimpleDto = new OfferStatusSimpleDto()
            {
                OfferStatusID = offerStatus.OfferStatusID,
                OfferStatusName = offerStatus.OfferStatusName,
                OfferStatusDisplayName = offerStatus.OfferStatusDisplayName
            };
            DoCustomSimpleDtoMappings(offerStatus, offerStatusSimpleDto);
            return offerStatusSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(OfferStatus offerStatus, OfferStatusSimpleDto offerStatusSimpleDto);
    }
}