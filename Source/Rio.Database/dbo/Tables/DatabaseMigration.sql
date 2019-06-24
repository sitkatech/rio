CREATE TABLE [dbo].[DatabaseMigration] (
    [DatabaseMigrationNumber]	INT				NOT NULL IDENTITY(1,1),
	[ReleaseScriptFileName]		VARCHAR(500)	NOT NULL,
	[DateMigrated]				DATETIME2		NOT NULL DEFAULT(GETUTCDATE()),
	[MigrationAuthorName]		VARCHAR(200)	NOT NULL, 
	[MigrationReason]			VARCHAR(MAX)	NULL
    CONSTRAINT [PK_DatabaseMigration_DatabaseMigrationNumber]	PRIMARY KEY CLUSTERED ([DatabaseMigrationNumber] ASC),	
    CONSTRAINT [UC_DatabaseMigration_ReleaseScriptFileName]		UNIQUE NONCLUSTERED ([ReleaseScriptFileName] ASC),
);
