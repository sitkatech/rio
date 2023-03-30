//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelUsageFileUpload]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelUsageFileUploadExtensionMethods
    {
        public static ParcelUsageFileUploadDto AsDto(this ParcelUsageFileUpload parcelUsageFileUpload)
        {
            var parcelUsageFileUploadDto = new ParcelUsageFileUploadDto()
            {
                ParcelUsageFileUploadID = parcelUsageFileUpload.ParcelUsageFileUploadID,
                User = parcelUsageFileUpload.User.AsDto(),
                UploadedFileName = parcelUsageFileUpload.UploadedFileName,
                UploadDate = parcelUsageFileUpload.UploadDate,
                PublishDate = parcelUsageFileUpload.PublishDate,
                MatchedRecordCount = parcelUsageFileUpload.MatchedRecordCount,
                UnmatchedParcelNumberCount = parcelUsageFileUpload.UnmatchedParcelNumberCount,
                NullParcelNumberCount = parcelUsageFileUpload.NullParcelNumberCount
            };
            DoCustomMappings(parcelUsageFileUpload, parcelUsageFileUploadDto);
            return parcelUsageFileUploadDto;
        }

        static partial void DoCustomMappings(ParcelUsageFileUpload parcelUsageFileUpload, ParcelUsageFileUploadDto parcelUsageFileUploadDto);

        public static ParcelUsageFileUploadSimpleDto AsSimpleDto(this ParcelUsageFileUpload parcelUsageFileUpload)
        {
            var parcelUsageFileUploadSimpleDto = new ParcelUsageFileUploadSimpleDto()
            {
                ParcelUsageFileUploadID = parcelUsageFileUpload.ParcelUsageFileUploadID,
                UserID = parcelUsageFileUpload.UserID,
                UploadedFileName = parcelUsageFileUpload.UploadedFileName,
                UploadDate = parcelUsageFileUpload.UploadDate,
                PublishDate = parcelUsageFileUpload.PublishDate,
                MatchedRecordCount = parcelUsageFileUpload.MatchedRecordCount,
                UnmatchedParcelNumberCount = parcelUsageFileUpload.UnmatchedParcelNumberCount,
                NullParcelNumberCount = parcelUsageFileUpload.NullParcelNumberCount
            };
            DoCustomSimpleDtoMappings(parcelUsageFileUpload, parcelUsageFileUploadSimpleDto);
            return parcelUsageFileUploadSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelUsageFileUpload parcelUsageFileUpload, ParcelUsageFileUploadSimpleDto parcelUsageFileUploadSimpleDto);
    }
}