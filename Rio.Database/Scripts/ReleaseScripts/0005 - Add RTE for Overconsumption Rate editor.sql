DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0005 - Add RTE for Overconsumption Rate editor'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;

    insert into dbo.CustomRichText(CustomRichTextTypeID, CustomRichTextContent)
    values (35, 'Instructions for setting the overconsumption rate for a given water year...')

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Jamie Quishenberry', @MigrationName, 'Add RTE for Overconsumption Rate editor'
END