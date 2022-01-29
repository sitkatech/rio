//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferRegistrationStatus]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferRegistrationStatusExtensionMethods
    {
        public static WaterTransferRegistrationStatusDto AsDto(this WaterTransferRegistrationStatus waterTransferRegistrationStatus)
        {
            var waterTransferRegistrationStatusDto = new WaterTransferRegistrationStatusDto()
            {
                WaterTransferRegistrationStatusID = waterTransferRegistrationStatus.WaterTransferRegistrationStatusID,
                WaterTransferRegistrationStatusName = waterTransferRegistrationStatus.WaterTransferRegistrationStatusName,
                WaterTransferRegistrationStatusDisplayName = waterTransferRegistrationStatus.WaterTransferRegistrationStatusDisplayName
            };
            DoCustomMappings(waterTransferRegistrationStatus, waterTransferRegistrationStatusDto);
            return waterTransferRegistrationStatusDto;
        }

        static partial void DoCustomMappings(WaterTransferRegistrationStatus waterTransferRegistrationStatus, WaterTransferRegistrationStatusDto waterTransferRegistrationStatusDto);

        public static WaterTransferRegistrationStatusSimpleDto AsSimpleDto(this WaterTransferRegistrationStatus waterTransferRegistrationStatus)
        {
            var waterTransferRegistrationStatusSimpleDto = new WaterTransferRegistrationStatusSimpleDto()
            {
                WaterTransferRegistrationStatusID = waterTransferRegistrationStatus.WaterTransferRegistrationStatusID,
                WaterTransferRegistrationStatusName = waterTransferRegistrationStatus.WaterTransferRegistrationStatusName,
                WaterTransferRegistrationStatusDisplayName = waterTransferRegistrationStatus.WaterTransferRegistrationStatusDisplayName
            };
            DoCustomSimpleDtoMappings(waterTransferRegistrationStatus, waterTransferRegistrationStatusSimpleDto);
            return waterTransferRegistrationStatusSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterTransferRegistrationStatus waterTransferRegistrationStatus, WaterTransferRegistrationStatusSimpleDto waterTransferRegistrationStatusSimpleDto);
    }
}