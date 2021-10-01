﻿using System.Transactions;
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
                TransactionAmount = parcelLedger.TransactionAmount,
                ParcelLedgerID = parcelLedger.ParcelLedgerID,
                TransactionDescription = parcelLedger.TransactionDescription
            };
        }
    }
}