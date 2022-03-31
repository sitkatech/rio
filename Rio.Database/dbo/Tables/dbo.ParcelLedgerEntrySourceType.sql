CREATE TABLE [dbo].[ParcelLedgerEntrySourceType](
	[ParcelLedgerEntrySourceTypeID] [int] NOT NULL CONSTRAINT [PK_ParcelLedgerEntrySourceType_SourceTypeID] PRIMARY KEY,
	[ParcelLedgerEntrySourceTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeName] UNIQUE,
	[ParcelLedgerEntrySourceTypeDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeDisplayName] UNIQUE,
)
