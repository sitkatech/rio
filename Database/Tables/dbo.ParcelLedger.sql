SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelLedger](
	[ParcelLedgerID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelID] [int] NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[TransactionTypeID] [int] NOT NULL,
	[TransactionAmount] [decimal](10, 4) NOT NULL,
	[WaterTypeID] [int] NULL,
	[TransactionDescription] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UserID] [int] NULL,
	[UserComment] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ParcelLedgerEntrySourceTypeID] [int] NOT NULL,
	[UploadedFileName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ParcelLedger_ParcelLedgerID] PRIMARY KEY CLUSTERED 
(
	[ParcelLedgerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ParcelLedger]  WITH CHECK ADD  CONSTRAINT [FK_ParcelLedger_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[ParcelLedger] CHECK CONSTRAINT [FK_ParcelLedger_Parcel_ParcelID]
GO
ALTER TABLE [dbo].[ParcelLedger]  WITH CHECK ADD  CONSTRAINT [FK_ParcelLedger_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeID] FOREIGN KEY([ParcelLedgerEntrySourceTypeID])
REFERENCES [dbo].[ParcelLedgerEntrySourceType] ([ParcelLedgerEntrySourceTypeID])
GO
ALTER TABLE [dbo].[ParcelLedger] CHECK CONSTRAINT [FK_ParcelLedger_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeID]
GO
ALTER TABLE [dbo].[ParcelLedger]  WITH CHECK ADD  CONSTRAINT [FK_ParcelLedger_TransactionType_TransactionTypeID] FOREIGN KEY([TransactionTypeID])
REFERENCES [dbo].[TransactionType] ([TransactionTypeID])
GO
ALTER TABLE [dbo].[ParcelLedger] CHECK CONSTRAINT [FK_ParcelLedger_TransactionType_TransactionTypeID]
GO
ALTER TABLE [dbo].[ParcelLedger]  WITH CHECK ADD  CONSTRAINT [FK_ParcelLedger_User_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ParcelLedger] CHECK CONSTRAINT [FK_ParcelLedger_User_UserID]
GO
ALTER TABLE [dbo].[ParcelLedger]  WITH CHECK ADD  CONSTRAINT [FK_ParcelLedger_WaterType_WaterTypeID] FOREIGN KEY([WaterTypeID])
REFERENCES [dbo].[WaterType] ([WaterTypeID])
GO
ALTER TABLE [dbo].[ParcelLedger] CHECK CONSTRAINT [FK_ParcelLedger_WaterType_WaterTypeID]