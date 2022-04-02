CREATE TABLE [dbo].[OpenETSyncResultType](
	[OpenETSyncResultTypeID] [int] NOT NULL CONSTRAINT [PK_OpenETSyncResultType_OpenETSyncResultTypeID] PRIMARY KEY,
	[OpenETSyncResultTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_OpenETSyncResultType_AK_OpenETSyncResultTypeName] UNIQUE,
	[OpenETSyncResultTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_OpenETSyncResultType_OpenETSyncResultTypeDisplayName] UNIQUE
)
