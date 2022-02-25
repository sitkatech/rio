CREATE TABLE [dbo].[AccountStatus](
	[AccountStatusID] [int] NOT NULL CONSTRAINT [PK_AccountStatus_AccountStatusID] PRIMARY KEY,
	[AccountStatusName] [varchar](20) NOT NULL CONSTRAINT [AK_AccountStatus_AccountStatusName] UNIQUE,
	[AccountStatusDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_AccountStatus_AccountStatusDisplayName] UNIQUE,
)
