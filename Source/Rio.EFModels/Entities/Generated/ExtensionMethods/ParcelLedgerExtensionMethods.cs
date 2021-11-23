//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelLedger]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelLedgerExtensionMethods
    {
        public static ParcelLedgerDto AsDto(this ParcelLedger parcelLedger)
        {
            var parcelLedgerDto = new ParcelLedgerDto()
            {
                ParcelLedgerID = parcelLedger.ParcelLedgerID,
                Parcel = parcelLedger.Parcel.AsDto(),
                TransactionDate = parcelLedger.TransactionDate,
                EffectiveDate = parcelLedger.EffectiveDate,
                TransactionType = parcelLedger.TransactionType.AsDto(),
                TransactionAmount = parcelLedger.TransactionAmount,
                WaterType = parcelLedger.WaterType?.AsDto(),
                TransactionDescription = parcelLedger.TransactionDescription,
                User = parcelLedger.User?.AsDto(),
                UserComment = parcelLedger.UserComment
            };
            DoCustomMappings(parcelLedger, parcelLedgerDto);
            return parcelLedgerDto;
        }

        static partial void DoCustomMappings(ParcelLedger parcelLedger, ParcelLedgerDto parcelLedgerDto);

        public static ParcelLedgerSimpleDto AsSimpleDto(this ParcelLedger parcelLedger)
        {
            var parcelLedgerSimpleDto = new ParcelLedgerSimpleDto()
            {
                ParcelLedgerID = parcelLedger.ParcelLedgerID,
                ParcelID = parcelLedger.ParcelID,
                TransactionDate = parcelLedger.TransactionDate,
                EffectiveDate = parcelLedger.EffectiveDate,
                TransactionTypeID = parcelLedger.TransactionTypeID,
                TransactionAmount = parcelLedger.TransactionAmount,
                WaterTypeID = parcelLedger.WaterTypeID,
                TransactionDescription = parcelLedger.TransactionDescription,
                UserID = parcelLedger.UserID,
                UserComment = parcelLedger.UserComment
            };
            DoCustomSimpleDtoMappings(parcelLedger, parcelLedgerSimpleDto);
            return parcelLedgerSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelLedger parcelLedger, ParcelLedgerSimpleDto parcelLedgerSimpleDto);
    }
}