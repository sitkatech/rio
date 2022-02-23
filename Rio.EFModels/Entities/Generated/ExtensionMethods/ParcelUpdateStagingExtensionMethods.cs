//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelUpdateStaging]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelUpdateStagingExtensionMethods
    {
        public static ParcelUpdateStagingDto AsDto(this ParcelUpdateStaging parcelUpdateStaging)
        {
            var parcelUpdateStagingDto = new ParcelUpdateStagingDto()
            {
                ParcelUpdateStagingID = parcelUpdateStaging.ParcelUpdateStagingID,
                ParcelNumber = parcelUpdateStaging.ParcelNumber,
                OwnerName = parcelUpdateStaging.OwnerName,
                ParcelGeometryText = parcelUpdateStaging.ParcelGeometryText,
                ParcelGeometry4326Text = parcelUpdateStaging.ParcelGeometry4326Text,
                HasConflict = parcelUpdateStaging.HasConflict
            };
            DoCustomMappings(parcelUpdateStaging, parcelUpdateStagingDto);
            return parcelUpdateStagingDto;
        }

        static partial void DoCustomMappings(ParcelUpdateStaging parcelUpdateStaging, ParcelUpdateStagingDto parcelUpdateStagingDto);

        public static ParcelUpdateStagingSimpleDto AsSimpleDto(this ParcelUpdateStaging parcelUpdateStaging)
        {
            var parcelUpdateStagingSimpleDto = new ParcelUpdateStagingSimpleDto()
            {
                ParcelUpdateStagingID = parcelUpdateStaging.ParcelUpdateStagingID,
                ParcelNumber = parcelUpdateStaging.ParcelNumber,
                OwnerName = parcelUpdateStaging.OwnerName,
                ParcelGeometryText = parcelUpdateStaging.ParcelGeometryText,
                ParcelGeometry4326Text = parcelUpdateStaging.ParcelGeometry4326Text,
                HasConflict = parcelUpdateStaging.HasConflict
            };
            DoCustomSimpleDtoMappings(parcelUpdateStaging, parcelUpdateStagingSimpleDto);
            return parcelUpdateStagingSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelUpdateStaging parcelUpdateStaging, ParcelUpdateStagingSimpleDto parcelUpdateStagingSimpleDto);
    }
}