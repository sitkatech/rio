//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelStatus]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelStatusExtensionMethods
    {
        public static ParcelStatusDto AsDto(this ParcelStatus parcelStatus)
        {
            var parcelStatusDto = new ParcelStatusDto()
            {
                ParcelStatusID = parcelStatus.ParcelStatusID,
                ParcelStatusName = parcelStatus.ParcelStatusName,
                ParcelStatusDisplayName = parcelStatus.ParcelStatusDisplayName
            };
            DoCustomMappings(parcelStatus, parcelStatusDto);
            return parcelStatusDto;
        }

        static partial void DoCustomMappings(ParcelStatus parcelStatus, ParcelStatusDto parcelStatusDto);

        public static ParcelStatusSimpleDto AsSimpleDto(this ParcelStatus parcelStatus)
        {
            var parcelStatusSimpleDto = new ParcelStatusSimpleDto()
            {
                ParcelStatusID = parcelStatus.ParcelStatusID,
                ParcelStatusName = parcelStatus.ParcelStatusName,
                ParcelStatusDisplayName = parcelStatus.ParcelStatusDisplayName
            };
            DoCustomSimpleDtoMappings(parcelStatus, parcelStatusSimpleDto);
            return parcelStatusSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelStatus parcelStatus, ParcelStatusSimpleDto parcelStatusSimpleDto);
    }
}