//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DisadvantagedCommunityStatus]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class DisadvantagedCommunityStatusExtensionMethods
    {
        public static DisadvantagedCommunityStatusDto AsDto(this DisadvantagedCommunityStatus disadvantagedCommunityStatus)
        {
            var disadvantagedCommunityStatusDto = new DisadvantagedCommunityStatusDto()
            {
                DisadvantagedCommunityStatusID = disadvantagedCommunityStatus.DisadvantagedCommunityStatusID,
                DisadvantagedCommunityStatusName = disadvantagedCommunityStatus.DisadvantagedCommunityStatusName,
                GeoServerLayerColor = disadvantagedCommunityStatus.GeoServerLayerColor
            };
            DoCustomMappings(disadvantagedCommunityStatus, disadvantagedCommunityStatusDto);
            return disadvantagedCommunityStatusDto;
        }

        static partial void DoCustomMappings(DisadvantagedCommunityStatus disadvantagedCommunityStatus, DisadvantagedCommunityStatusDto disadvantagedCommunityStatusDto);

        public static DisadvantagedCommunityStatusSimpleDto AsSimpleDto(this DisadvantagedCommunityStatus disadvantagedCommunityStatus)
        {
            var disadvantagedCommunityStatusSimpleDto = new DisadvantagedCommunityStatusSimpleDto()
            {
                DisadvantagedCommunityStatusID = disadvantagedCommunityStatus.DisadvantagedCommunityStatusID,
                DisadvantagedCommunityStatusName = disadvantagedCommunityStatus.DisadvantagedCommunityStatusName,
                GeoServerLayerColor = disadvantagedCommunityStatus.GeoServerLayerColor
            };
            DoCustomSimpleDtoMappings(disadvantagedCommunityStatus, disadvantagedCommunityStatusSimpleDto);
            return disadvantagedCommunityStatusSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(DisadvantagedCommunityStatus disadvantagedCommunityStatus, DisadvantagedCommunityStatusSimpleDto disadvantagedCommunityStatusSimpleDto);
    }
}