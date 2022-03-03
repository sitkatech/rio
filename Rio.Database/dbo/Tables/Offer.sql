CREATE TABLE [dbo].[Offer](
	[OfferID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Offer_OfferID] PRIMARY KEY,
	[TradeID] [int] NOT NULL CONSTRAINT [FK_Offer_Trade_TradeID] FOREIGN KEY REFERENCES [dbo].[Trade] ([TradeID]),
	[OfferDate] [datetime] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[OfferStatusID] [int] NOT NULL CONSTRAINT [FK_Offer_OfferStatus_OfferStatusID] FOREIGN KEY REFERENCES [dbo].[OfferStatus] ([OfferStatusID]),
	[OfferNotes] [varchar](2000) NULL,
	[CreateAccountID] [int] NOT NULL CONSTRAINT [FK_Offer_User_CreateAccountID_AccountID] FOREIGN KEY REFERENCES [dbo].[Account] ([AccountID]),
)