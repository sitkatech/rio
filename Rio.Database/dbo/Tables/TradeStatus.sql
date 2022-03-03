CREATE TABLE [dbo].[TradeStatus](
	[TradeStatusID] [int] NOT NULL CONSTRAINT [PK_TradeStatus_TradeStatusID] PRIMARY KEY,
	[TradeStatusName] [varchar](100) NOT NULL CONSTRAINT [AK_TradeStatus_TradeStatusName] UNIQUE,
	[TradeStatusDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_TradeStatus_TradeStatusDisplayName] UNIQUE,
)
