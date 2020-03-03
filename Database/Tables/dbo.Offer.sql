SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Offer](
	[OfferID] [int] IDENTITY(1,1) NOT NULL,
	[TradeID] [int] NOT NULL,
	[OfferDate] [datetime] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[OfferStatusID] [int] NOT NULL,
	[OfferNotes] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CreateAccountID] [int] NOT NULL,
 CONSTRAINT [PK_Offer_OfferID] PRIMARY KEY CLUSTERED 
(
	[OfferID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_OfferStatus_OfferStatusID] FOREIGN KEY([OfferStatusID])
REFERENCES [dbo].[OfferStatus] ([OfferStatusID])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_OfferStatus_OfferStatusID]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_Trade_TradeID] FOREIGN KEY([TradeID])
REFERENCES [dbo].[Trade] ([TradeID])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_Trade_TradeID]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_User_CreateAccountID_AccountID] FOREIGN KEY([CreateAccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_User_CreateAccountID_AccountID]