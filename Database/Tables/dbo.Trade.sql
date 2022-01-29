SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Trade](
	[TradeID] [int] IDENTITY(1,1) NOT NULL,
	[PostingID] [int] NOT NULL,
	[TradeDate] [datetime] NOT NULL,
	[TradeStatusID] [int] NOT NULL,
	[CreateAccountID] [int] NOT NULL,
	[TradeNumber] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Trade_TradeID] PRIMARY KEY CLUSTERED 
(
	[TradeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Trade]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Account_CreateAccountID_AccountID] FOREIGN KEY([CreateAccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
ALTER TABLE [dbo].[Trade] CHECK CONSTRAINT [FK_Trade_Account_CreateAccountID_AccountID]
GO
ALTER TABLE [dbo].[Trade]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Posting_PostingID] FOREIGN KEY([PostingID])
REFERENCES [dbo].[Posting] ([PostingID])
GO
ALTER TABLE [dbo].[Trade] CHECK CONSTRAINT [FK_Trade_Posting_PostingID]
GO
ALTER TABLE [dbo].[Trade]  WITH CHECK ADD  CONSTRAINT [FK_Trade_TradeStatus_TradeStatusID] FOREIGN KEY([TradeStatusID])
REFERENCES [dbo].[TradeStatus] ([TradeStatusID])
GO
ALTER TABLE [dbo].[Trade] CHECK CONSTRAINT [FK_Trade_TradeStatus_TradeStatusID]