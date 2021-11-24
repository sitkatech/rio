//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferRegistrationParcel]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferRegistrationParcelExtensionMethods
    {
        public static WaterTransferRegistrationParcelDto AsDto(this WaterTransferRegistrationParcel waterTransferRegistrationParcel)
        {
            var waterTransferRegistrationParcelDto = new WaterTransferRegistrationParcelDto()
            {
                WaterTransferRegistrationParcelID = waterTransferRegistrationParcel.WaterTransferRegistrationParcelID,
                WaterTransferRegistration = waterTransferRegistrationParcel.WaterTransferRegistration.AsSimpleDto(),
                Parcel = waterTransferRegistrationParcel.Parcel.AsDto(),
                AcreFeetTransferred = waterTransferRegistrationParcel.AcreFeetTransferred
            };
            DoCustomMappings(waterTransferRegistrationParcel, waterTransferRegistrationParcelDto);
            return waterTransferRegistrationParcelDto;
        }

        static partial void DoCustomMappings(WaterTransferRegistrationParcel waterTransferRegistrationParcel, WaterTransferRegistrationParcelDto waterTransferRegistrationParcelDto);

        public static WaterTransferRegistrationParcelSimpleDto AsSimpleDto(this WaterTransferRegistrationParcel waterTransferRegistrationParcel)
        {
            var waterTransferRegistrationParcelSimpleDto = new WaterTransferRegistrationParcelSimpleDto()
            {
                WaterTransferRegistrationParcelID = waterTransferRegistrationParcel.WaterTransferRegistrationParcelID,
                WaterTransferRegistrationID = waterTransferRegistrationParcel.WaterTransferRegistrationID,
                ParcelID = waterTransferRegistrationParcel.ParcelID,
                AcreFeetTransferred = waterTransferRegistrationParcel.AcreFeetTransferred
            };
            DoCustomSimpleDtoMappings(waterTransferRegistrationParcel, waterTransferRegistrationParcelSimpleDto);
            return waterTransferRegistrationParcelSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterTransferRegistrationParcel waterTransferRegistrationParcel, WaterTransferRegistrationParcelSimpleDto waterTransferRegistrationParcelSimpleDto);
    }
}