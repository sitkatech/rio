CREATE TABLE [dbo].[WaterTransfer](
	[WaterTransferID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterTransfer_WaterTransferID] PRIMARY KEY,
	[TransferDate] [datetime] NOT NULL,
	[AcreFeetTransferred] [int] NOT NULL,
	[OfferID] [int] NOT NULL CONSTRAINT [FK_WaterTransfer_Offer_OfferID] FOREIGN KEY REFERENCES [dbo].[Offer] ([OfferID]),
	[Notes] [varchar](2000) NULL
)