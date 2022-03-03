CREATE TABLE [dbo].[ParcelLedger](
	[ParcelLedgerID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelLedger_ParcelLedgerID] PRIMARY KEY,
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_ParcelLedger_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[TransactionDate] [datetime] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[TransactionTypeID] [int] NOT NULL CONSTRAINT [FK_ParcelLedger_TransactionType_TransactionTypeID] FOREIGN KEY REFERENCES [dbo].[TransactionType] ([TransactionTypeID]),
	[TransactionAmount] [decimal](10, 4) NOT NULL,
	[WaterTypeID] [int] NULL CONSTRAINT [FK_ParcelLedger_WaterType_WaterTypeID] FOREIGN KEY REFERENCES [dbo].[WaterType] ([WaterTypeID]),
	[TransactionDescription] [varchar](200) NOT NULL,
	[UserID] [int] NULL CONSTRAINT [FK_ParcelLedger_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[UserComment] [varchar](max) NULL,
	[ParcelLedgerEntrySourceTypeID] [int] NOT NULL CONSTRAINT [FK_ParcelLedger_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeID] FOREIGN KEY REFERENCES [dbo].[ParcelLedgerEntrySourceType] ([ParcelLedgerEntrySourceTypeID]),
	[UploadedFileName] [varchar](100) NULL,
)