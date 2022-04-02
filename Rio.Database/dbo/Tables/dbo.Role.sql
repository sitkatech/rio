CREATE TABLE [dbo].[Role](
	[RoleID] [int] NOT NULL CONSTRAINT [PK_Role_RoleID] PRIMARY KEY,
	[RoleName] [varchar](100) NOT NULL CONSTRAINT [AK_Role_RoleName] UNIQUE,
	[RoleDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_Role_RoleDisplayName] UNIQUE,
	[RoleDescription] [varchar](255) NULL,
	[SortOrder] [int] NOT NULL
)
