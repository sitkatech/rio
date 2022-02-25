CREATE TABLE [dbo].[Trade](
	[TradeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Trade_TradeID] PRIMARY KEY,
	[PostingID] [int] NOT NULL CONSTRAINT [FK_Trade_Posting_PostingID] FOREIGN KEY REFERENCES [dbo].[Posting] ([PostingID]),
	[TradeDate] [datetime] NOT NULL,
	[TradeStatusID] [int] NOT NULL CONSTRAINT [FK_Trade_TradeStatus_TradeStatusID] FOREIGN KEY REFERENCES [dbo].[TradeStatus] ([TradeStatusID]),
	[CreateAccountID] [int] NOT NULL CONSTRAINT [FK_Trade_Account_CreateAccountID_AccountID] FOREIGN KEY REFERENCES [dbo].[Account] ([AccountID]),
	[TradeNumber] [varchar](50) NULL
)