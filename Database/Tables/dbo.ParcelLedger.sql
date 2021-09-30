SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelLedger](
	[ParcelLedgerID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelID] [int] NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[TransactionTypeID] [int] NOT NULL,
	[TransactionAmount] [decimal](10, 4) NOT NULL,
	[TransactionDescription] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ParcelLedger_ParcelLedgerID] PRIMARY KEY CLUSTERED 
(
	[ParcelLedgerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ParcelLedger_ParcelID_TransactionDate_TransactionTypeID] UNIQUE NONCLUSTERED 
(
	[ParcelID] ASC,
	[TransactionDate] ASC,
	[TransactionTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ParcelLedger]  WITH CHECK ADD  CONSTRAINT [FK_ParcelLedger_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[ParcelLedger] CHECK CONSTRAINT [FK_ParcelLedger_Parcel_ParcelID]
GO
ALTER TABLE [dbo].[ParcelLedger]  WITH CHECK ADD  CONSTRAINT [FK_ParcelLedger_TransactionType_TransactionTypeID] FOREIGN KEY([TransactionTypeID])
REFERENCES [dbo].[TransactionType] ([TransactionTypeID])
GO
ALTER TABLE [dbo].[ParcelLedger] CHECK CONSTRAINT [FK_ParcelLedger_TransactionType_TransactionTypeID]