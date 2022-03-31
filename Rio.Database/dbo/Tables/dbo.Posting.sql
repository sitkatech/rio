CREATE TABLE [dbo].[Posting](
	[PostingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Posting_PostingID] PRIMARY KEY,
	[PostingTypeID] [int] NOT NULL CONSTRAINT [FK_Posting_PostingType_PostingTypeID] FOREIGN KEY REFERENCES [dbo].[PostingType] ([PostingTypeID]),
	[PostingDate] [datetime] NOT NULL,
	[CreateAccountID] [int] NOT NULL CONSTRAINT [FK_Posting_Account_CreateAccountID_AccountID] FOREIGN KEY REFERENCES [dbo].[Account] ([AccountID]),
	[Quantity] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[PostingDescription] [varchar](2000) NULL,
	[PostingStatusID] [int] NOT NULL CONSTRAINT [FK_Posting_PostingStatus_PostingStatusID] FOREIGN KEY REFERENCES [dbo].[PostingStatus] ([PostingStatusID]),
	[AvailableQuantity] [int] NOT NULL,
	[CreateUserID] [int] NULL CONSTRAINT [FK_Posting_User_CreateUserID_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID])
)