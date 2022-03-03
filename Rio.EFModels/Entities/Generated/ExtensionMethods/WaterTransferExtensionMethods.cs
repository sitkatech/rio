//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransfer]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferExtensionMethods
    {
        public static WaterTransferDto AsDto(this WaterTransfer waterTransfer)
        {
            var waterTransferDto = new WaterTransferDto()
            {
                WaterTransferID = waterTransfer.WaterTransferID,
                TransferDate = waterTransfer.TransferDate,
                AcreFeetTransferred = waterTransfer.AcreFeetTransferred,
                Offer = waterTransfer.Offer.AsDto(),
                Notes = waterTransfer.Notes
            };
            DoCustomMappings(waterTransfer, waterTransferDto);
            return waterTransferDto;
        }

        static partial void DoCustomMappings(WaterTransfer waterTransfer, WaterTransferDto waterTransferDto);

        public static WaterTransferSimpleDto AsSimpleDto(this WaterTransfer waterTransfer)
        {
            var waterTransferSimpleDto = new WaterTransferSimpleDto()
            {
                WaterTransferID = waterTransfer.WaterTransferID,
                TransferDate = waterTransfer.TransferDate,
                AcreFeetTransferred = waterTransfer.AcreFeetTransferred,
                OfferID = waterTransfer.OfferID,
                Notes = waterTransfer.Notes
            };
            DoCustomSimpleDtoMappings(waterTransfer, waterTransferSimpleDto);
            return waterTransferSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(WaterTransfer waterTransfer, WaterTransferSimpleDto waterTransferSimpleDto);
    }
}