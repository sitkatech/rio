DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0004 - Add RTE for Usage CSV Upload'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;

    insert into dbo.CustomRichText(CustomRichTextTypeID, CustomRichTextContent)
    values (33, 'Instructions for uploading usage transactions via CSV...'),
        (34, 'Instructions for previewing and approving usage transactions...')

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Jamie Quishenberry', @MigrationName, '0004 - Add RTE for Usage CSV Upload'
END