//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelUsageStaging]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelUsageStagingExtensionMethods
    {
        public static ParcelUsageStagingDto AsDto(this ParcelUsageStaging parcelUsageStaging)
        {
            var parcelUsageStagingDto = new ParcelUsageStagingDto()
            {
                ParcelUsageStagingID = parcelUsageStaging.ParcelUsageStagingID,
                Parcel = parcelUsageStaging.Parcel.AsDto(),
                ParcelNumber = parcelUsageStaging.ParcelNumber,
                ReportedDate = parcelUsageStaging.ReportedDate,
                ReportedValue = parcelUsageStaging.ReportedValue,
                ReportedValueInAcreFeet = parcelUsageStaging.ReportedValueInAcreFeet,
                LastUpdateDate = parcelUsageStaging.LastUpdateDate,
                UploadedFileName = parcelUsageStaging.UploadedFileName,
                User = parcelUsageStaging.User.AsDto()
            };
            DoCustomMappings(parcelUsageStaging, parcelUsageStagingDto);
            return parcelUsageStagingDto;
        }

        static partial void DoCustomMappings(ParcelUsageStaging parcelUsageStaging, ParcelUsageStagingDto parcelUsageStagingDto);

        public static ParcelUsageStagingSimpleDto AsSimpleDto(this ParcelUsageStaging parcelUsageStaging)
        {
            var parcelUsageStagingSimpleDto = new ParcelUsageStagingSimpleDto()
            {
                ParcelUsageStagingID = parcelUsageStaging.ParcelUsageStagingID,
                ParcelID = parcelUsageStaging.ParcelID,
                ParcelNumber = parcelUsageStaging.ParcelNumber,
                ReportedDate = parcelUsageStaging.ReportedDate,
                ReportedValue = parcelUsageStaging.ReportedValue,
                ReportedValueInAcreFeet = parcelUsageStaging.ReportedValueInAcreFeet,
                LastUpdateDate = parcelUsageStaging.LastUpdateDate,
                UploadedFileName = parcelUsageStaging.UploadedFileName,
                UserID = parcelUsageStaging.UserID
            };
            DoCustomSimpleDtoMappings(parcelUsageStaging, parcelUsageStagingSimpleDto);
            return parcelUsageStagingSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelUsageStaging parcelUsageStaging, ParcelUsageStagingSimpleDto parcelUsageStagingSimpleDto);
    }
}