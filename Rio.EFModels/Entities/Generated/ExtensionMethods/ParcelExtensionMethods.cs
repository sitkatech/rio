//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Parcel]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelExtensionMethods
    {
        public static ParcelDto AsDto(this Parcel parcel)
        {
            var parcelDto = new ParcelDto()
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelAreaInSquareFeet = parcel.ParcelAreaInSquareFeet,
                ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                ParcelStatus = parcel.ParcelStatus.AsDto(),
                InactivateDate = parcel.InactivateDate
            };
            DoCustomMappings(parcel, parcelDto);
            return parcelDto;
        }

        static partial void DoCustomMappings(Parcel parcel, ParcelDto parcelDto);

        public static ParcelSimpleDto AsSimpleDto(this Parcel parcel)
        {
            var parcelSimpleDto = new ParcelSimpleDto()
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelAreaInSquareFeet = parcel.ParcelAreaInSquareFeet,
                ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                ParcelStatusID = parcel.ParcelStatusID,
                InactivateDate = parcel.InactivateDate
            };
            DoCustomSimpleDtoMappings(parcel, parcelSimpleDto);
            return parcelSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Parcel parcel, ParcelSimpleDto parcelSimpleDto);
    }
}