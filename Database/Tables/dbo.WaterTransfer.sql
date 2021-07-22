SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WaterTransfer](
	[WaterTransferID] [int] IDENTITY(1,1) NOT NULL,
	[TransferDate] [datetime] NOT NULL,
	[AcreFeetTransferred] [int] NOT NULL,
	[OfferID] [int] NOT NULL,
	[Notes] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_WaterTransfer_WaterTransferID] PRIMARY KEY CLUSTERED 
(
	[WaterTransferID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[WaterTransfer]  WITH CHECK ADD  CONSTRAINT [FK_WaterTransfer_Offer_OfferID] FOREIGN KEY([OfferID])
REFERENCES [dbo].[Offer] ([OfferID])
GO
ALTER TABLE [dbo].[WaterTransfer] CHECK CONSTRAINT [FK_WaterTransfer_Offer_OfferID]