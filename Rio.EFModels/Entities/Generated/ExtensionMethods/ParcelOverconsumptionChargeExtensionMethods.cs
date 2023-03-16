//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelOverconsumptionCharge]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelOverconsumptionChargeExtensionMethods
    {
        public static ParcelOverconsumptionChargeDto AsDto(this ParcelOverconsumptionCharge parcelOverconsumptionCharge)
        {
            var parcelOverconsumptionChargeDto = new ParcelOverconsumptionChargeDto()
            {
                ParcelOverconsumptionChargeID = parcelOverconsumptionCharge.ParcelOverconsumptionChargeID,
                Parcel = parcelOverconsumptionCharge.Parcel.AsDto(),
                WaterYear = parcelOverconsumptionCharge.WaterYear.AsDto(),
                OverconsumptionAmount = parcelOverconsumptionCharge.OverconsumptionAmount,
                OverconsumptionCharge = parcelOverconsumptionCharge.OverconsumptionCharge
            };
            DoCustomMappings(parcelOverconsumptionCharge, parcelOverconsumptionChargeDto);
            return parcelOverconsumptionChargeDto;
        }

        static partial void DoCustomMappings(ParcelOverconsumptionCharge parcelOverconsumptionCharge, ParcelOverconsumptionChargeDto parcelOverconsumptionChargeDto);

        public static ParcelOverconsumptionChargeSimpleDto AsSimpleDto(this ParcelOverconsumptionCharge parcelOverconsumptionCharge)
        {
            var parcelOverconsumptionChargeSimpleDto = new ParcelOverconsumptionChargeSimpleDto()
            {
                ParcelOverconsumptionChargeID = parcelOverconsumptionCharge.ParcelOverconsumptionChargeID,
                ParcelID = parcelOverconsumptionCharge.ParcelID,
                WaterYearID = parcelOverconsumptionCharge.WaterYearID,
                OverconsumptionAmount = parcelOverconsumptionCharge.OverconsumptionAmount,
                OverconsumptionCharge = parcelOverconsumptionCharge.OverconsumptionCharge
            };
            DoCustomSimpleDtoMappings(parcelOverconsumptionCharge, parcelOverconsumptionChargeSimpleDto);
            return parcelOverconsumptionChargeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelOverconsumptionCharge parcelOverconsumptionCharge, ParcelOverconsumptionChargeSimpleDto parcelOverconsumptionChargeSimpleDto);
    }
}