//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelLedger]
namespace Rio.EFModels.Entities
{
    public partial class ParcelLedger
    {
        public TransactionType TransactionType => TransactionType.AllLookupDictionary[TransactionTypeID];
        public ParcelLedgerEntrySourceType ParcelLedgerEntrySourceType => ParcelLedgerEntrySourceType.AllLookupDictionary[ParcelLedgerEntrySourceTypeID];
    }
}