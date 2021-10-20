using System.Transactions;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public static class ParcelLedgerExtensionMethods
    {
        public static ParcelLedgerDto AsDto(this ParcelLedger parcelLedger)
        {
            return new ParcelLedgerDto()
            {
                TransactionType = parcelLedger.TransactionType.AsDto(),
                ParcelID = parcelLedger.ParcelID,
                TransactionDate = parcelLedger.TransactionDate,
                EffectiveDate = parcelLedger.EffectiveDate,
                TransactionAmount = parcelLedger.TransactionAmount,
                ParcelLedgerID = parcelLedger.ParcelLedgerID,
                WaterType = parcelLedger.WaterType?.AsDto(),
                TransactionDescription = parcelLedger.TransactionDescription
            };
        }
    }
}