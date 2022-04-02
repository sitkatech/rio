CREATE TABLE [dbo].[AccountUser](
	[AccountUserID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AccountUser_AccountUserID] PRIMARY KEY,
	[UserID] [int] NOT NULL CONSTRAINT [FK_AccountUser_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[AccountID] [int] NOT NULL CONSTRAINT [FK_AccountUser_Account_AccountID] FOREIGN KEY REFERENCES [dbo].[Account] ([AccountID]),
)