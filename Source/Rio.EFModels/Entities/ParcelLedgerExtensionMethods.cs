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
                TransactionTypeID = parcelLedger.TransactionTypeID,
                ParcelID = parcelLedger.ParcelID,
                TransactionDate = parcelLedger.TransactionDate,
                EffectiveDate = parcelLedger.EffectiveDate,
                TransactionAmount = parcelLedger.TransactionAmount,
                ParcelLedgerID = parcelLedger.ParcelLedgerID,
                WaterTypeID = parcelLedger.WaterTypeID,
                TransactionDescription = parcelLedger.TransactionDescription
            };
        }
    }
}