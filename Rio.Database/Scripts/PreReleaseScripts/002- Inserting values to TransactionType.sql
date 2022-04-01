DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '002- Inserting values to TransactionType'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
	
	PRINT @MigrationName;

	if exists(select c.column_id from sys.columns c where c.[name] = 'TransactionTypeDisplayName')
	begin 
		update dbo.TransactionType
		set TransactionTypeDisplayName = TransactionTypeName
	end
	
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Ray Lee', @MigrationName, '002- Inserting values to TransactionType'
END