CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_User_UserID] PRIMARY KEY,
	[UserGuid] [uniqueidentifier] NULL,
	[FirstName] [varchar](100) NOT NULL,
	[LastName] [varchar](100) NOT NULL,
	[Email] [varchar](255) NOT NULL CONSTRAINT [AK_User_Email] UNIQUE,
	[Phone] [varchar](30) NULL,
	[RoleID] [int] NOT NULL CONSTRAINT [FK_User_Role_RoleID] FOREIGN KEY REFERENCES [dbo].[Role] ([RoleID]),
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[LastActivityDate] [datetime] NULL,
	[DisclaimerAcknowledgedDate] [datetime] NULL,
	[ReceiveSupportEmails] [bit] NOT NULL,
	[LoginName] [varchar](128) NULL,
	[Company] [varchar](100) NULL
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [AK_User_UserGuid] ON [dbo].[User]
(
	[UserGuid] ASC
)
WHERE ([UserGuid] IS NOT NULL)
GO
