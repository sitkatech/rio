//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferRegistration]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferRegistrationExtensionMethods
    {
        public static WaterTransferRegistrationDto AsDto(this WaterTransferRegistration waterTransferRegistration)
        {
            var waterTransferRegistrationDto = new WaterTransferRegistrationDto()
            {
                WaterTransferRegistrationID = waterTransferRegistration.WaterTransferRegistrationID,
                WaterTransfer = waterTransferRegistration.WaterTransfer.AsDto(),
                WaterTransferType = waterTransferRegistration.WaterTransferType.AsDto(),
                Account = waterTransferRegistration.Account.AsDto(),
                WaterTransferRegistrationStatus = waterTransferRegistration.WaterTransferRegistrationStatus.AsDto(),
                StatusDate = waterTransferRegistration.StatusDate
            };
            DoCustomMappings(waterTransferRegistration, waterTransferRegistrationDto);
            return waterTransferRegistrationDto;
        }

        static partial void DoCustomMappings(WaterTransferRegistration waterTransferRegistration, WaterTransferRegistrationDto waterTransferRegistrationDto);

        public static WaterTransferRegistrationSimpleDto AsSimpleDto(this WaterTransferRegistration waterTransferRegistration)
        {
            var waterTransferRegistrationSimpleDto = new WaterTransferRegistrationSimpleDto()
            {
                WaterTransferRegistrationID = waterTransferRegistration.WaterTransferRegistrationID,
                WaterTransferID = waterTransferRegistration.WaterTransferID,
                WaterTransferTypeID = waterTransferRegistration.WaterTransferTypeID,
                AccountID = waterTransferRegistration.AccountID,
                WaterTransferRegistrationStatusID = waterTransferRegistration.WaterTransferRegistrationStatusID,
                StatusDate = waterTransferRegistration.StatusDate
            };
            DoCustomSimpleDtoMappings(waterTransferRegistration, waterTransferRegistrationSimpleDto);
            return waterTransferRegistrationSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterTransferRegistration waterTransferRegistration, WaterTransferRegistrationSimpleDto waterTransferRegistrationSimpleDto);
    }
}