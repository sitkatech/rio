//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelTag]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelTagExtensionMethods
    {
        public static ParcelTagDto AsDto(this ParcelTag parcelTag)
        {
            var parcelTagDto = new ParcelTagDto()
            {
                ParcelTagID = parcelTag.ParcelTagID,
                Parcel = parcelTag.Parcel.AsDto(),
                Tag = parcelTag.Tag.AsDto()
            };
            DoCustomMappings(parcelTag, parcelTagDto);
            return parcelTagDto;
        }

        static partial void DoCustomMappings(ParcelTag parcelTag, ParcelTagDto parcelTagDto);

        public static ParcelTagSimpleDto AsSimpleDto(this ParcelTag parcelTag)
        {
            var parcelTagSimpleDto = new ParcelTagSimpleDto()
            {
                ParcelTagID = parcelTag.ParcelTagID,
                ParcelID = parcelTag.ParcelID,
                TagID = parcelTag.TagID
            };
            DoCustomSimpleDtoMappings(parcelTag, parcelTagSimpleDto);
            return parcelTagSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelTag parcelTag, ParcelTagSimpleDto parcelTagSimpleDto);
    }
}