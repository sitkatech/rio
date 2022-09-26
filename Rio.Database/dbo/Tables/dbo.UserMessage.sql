CREATE TABLE [dbo].[UserMessage](
	[UserMessageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserMessage_UserMessageID] PRIMARY KEY,
	[CreateUserID] [int] NOT NULL CONSTRAINT [FK_UserMessage_User_CreateUserID_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	RecipientUserID [int] NOT NULL CONSTRAINT [FK_UserMessage_User_RecipientUserID_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[CreateDate] [datetime] NOT NULL,
	[Message] [varchar](5000) NOT NULL
)