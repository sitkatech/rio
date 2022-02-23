//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DisadvantagedCommunity]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class DisadvantagedCommunityExtensionMethods
    {
        public static DisadvantagedCommunityDto AsDto(this DisadvantagedCommunity disadvantagedCommunity)
        {
            var disadvantagedCommunityDto = new DisadvantagedCommunityDto()
            {
                DisadvantagedCommunityID = disadvantagedCommunity.DisadvantagedCommunityID,
                DisadvantagedCommunityName = disadvantagedCommunity.DisadvantagedCommunityName,
                LSADCode = disadvantagedCommunity.LSADCode,
                DisadvantagedCommunityStatus = disadvantagedCommunity.DisadvantagedCommunityStatus.AsDto()
            };
            DoCustomMappings(disadvantagedCommunity, disadvantagedCommunityDto);
            return disadvantagedCommunityDto;
        }

        static partial void DoCustomMappings(DisadvantagedCommunity disadvantagedCommunity, DisadvantagedCommunityDto disadvantagedCommunityDto);

        public static DisadvantagedCommunitySimpleDto AsSimpleDto(this DisadvantagedCommunity disadvantagedCommunity)
        {
            var disadvantagedCommunitySimpleDto = new DisadvantagedCommunitySimpleDto()
            {
                DisadvantagedCommunityID = disadvantagedCommunity.DisadvantagedCommunityID,
                DisadvantagedCommunityName = disadvantagedCommunity.DisadvantagedCommunityName,
                LSADCode = disadvantagedCommunity.LSADCode,
                DisadvantagedCommunityStatusID = disadvantagedCommunity.DisadvantagedCommunityStatusID
            };
            DoCustomSimpleDtoMappings(disadvantagedCommunity, disadvantagedCommunitySimpleDto);
            return disadvantagedCommunitySimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(DisadvantagedCommunity disadvantagedCommunity, DisadvantagedCommunitySimpleDto disadvantagedCommunitySimpleDto);
    }
}