SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Posting](
	[PostingID] [int] IDENTITY(1,1) NOT NULL,
	[PostingTypeID] [int] NOT NULL,
	[PostingDate] [datetime] NOT NULL,
	[CreateAccountID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[PostingDescription] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PostingStatusID] [int] NOT NULL,
	[AvailableQuantity] [int] NOT NULL,
	[CreateUserID] [int] NULL,
 CONSTRAINT [PK_Posting_PostingID] PRIMARY KEY CLUSTERED 
(
	[PostingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Posting]  WITH CHECK ADD  CONSTRAINT [FK_Posting_Account_CreateAccountID_AccountID] FOREIGN KEY([CreateAccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
ALTER TABLE [dbo].[Posting] CHECK CONSTRAINT [FK_Posting_Account_CreateAccountID_AccountID]
GO
ALTER TABLE [dbo].[Posting]  WITH CHECK ADD  CONSTRAINT [FK_Posting_PostingStatus_PostingStatusID] FOREIGN KEY([PostingStatusID])
REFERENCES [dbo].[PostingStatus] ([PostingStatusID])
GO
ALTER TABLE [dbo].[Posting] CHECK CONSTRAINT [FK_Posting_PostingStatus_PostingStatusID]
GO
ALTER TABLE [dbo].[Posting]  WITH CHECK ADD  CONSTRAINT [FK_Posting_PostingType_PostingTypeID] FOREIGN KEY([PostingTypeID])
REFERENCES [dbo].[PostingType] ([PostingTypeID])
GO
ALTER TABLE [dbo].[Posting] CHECK CONSTRAINT [FK_Posting_PostingType_PostingTypeID]
GO
ALTER TABLE [dbo].[Posting]  WITH CHECK ADD  CONSTRAINT [FK_Posting_User_CreateUserID_UserID] FOREIGN KEY([CreateUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Posting] CHECK CONSTRAINT [FK_Posting_User_CreateUserID_UserID]