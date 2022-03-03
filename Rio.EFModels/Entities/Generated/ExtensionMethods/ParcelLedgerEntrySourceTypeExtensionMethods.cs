//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelLedgerEntrySourceType]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelLedgerEntrySourceTypeExtensionMethods
    {
        public static ParcelLedgerEntrySourceTypeDto AsDto(this ParcelLedgerEntrySourceType parcelLedgerEntrySourceType)
        {
            var parcelLedgerEntrySourceTypeDto = new ParcelLedgerEntrySourceTypeDto()
            {
                ParcelLedgerEntrySourceTypeID = parcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID,
                ParcelLedgerEntrySourceTypeName = parcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeName,
                ParcelLedgerEntrySourceTypeDisplayName = parcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeDisplayName
            };
            DoCustomMappings(parcelLedgerEntrySourceType, parcelLedgerEntrySourceTypeDto);
            return parcelLedgerEntrySourceTypeDto;
        }

        static partial void DoCustomMappings(ParcelLedgerEntrySourceType parcelLedgerEntrySourceType, ParcelLedgerEntrySourceTypeDto parcelLedgerEntrySourceTypeDto);

        public static ParcelLedgerEntrySourceTypeSimpleDto AsSimpleDto(this ParcelLedgerEntrySourceType parcelLedgerEntrySourceType)
        {
            var parcelLedgerEntrySourceTypeSimpleDto = new ParcelLedgerEntrySourceTypeSimpleDto()
            {
                ParcelLedgerEntrySourceTypeID = parcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID,
                ParcelLedgerEntrySourceTypeName = parcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeName,
                ParcelLedgerEntrySourceTypeDisplayName = parcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeDisplayName
            };
            DoCustomSimpleDtoMappings(parcelLedgerEntrySourceType, parcelLedgerEntrySourceTypeSimpleDto);
            return parcelLedgerEntrySourceTypeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelLedgerEntrySourceType parcelLedgerEntrySourceType, ParcelLedgerEntrySourceTypeSimpleDto parcelLedgerEntrySourceTypeSimpleDto);
    }
}