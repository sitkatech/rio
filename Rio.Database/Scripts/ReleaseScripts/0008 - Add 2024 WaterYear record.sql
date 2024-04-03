DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0008 - Add 2024 WaterYear record'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;

	insert into dbo.WaterYear([Year])
	values (2024)

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Jamie Quishenberry', @MigrationName, 'Add 2024 WaterYear record'
END