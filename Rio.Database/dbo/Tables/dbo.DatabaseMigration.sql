CREATE TABLE [dbo].[DatabaseMigration] (
    [DatabaseMigrationNumber] INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_DatabaseMigration_DatabaseMigrationNumber] PRIMARY KEY,
	[ReleaseScriptFileName]	VARCHAR(500) NOT NULL CONSTRAINT [AK_DatabaseMigration_ReleaseScriptFileName] UNIQUE,
	[DateMigrated] DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
	[MigrationAuthorName] VARCHAR(200) NOT NULL, 
	[MigrationReason] VARCHAR(MAX) NULL
)

