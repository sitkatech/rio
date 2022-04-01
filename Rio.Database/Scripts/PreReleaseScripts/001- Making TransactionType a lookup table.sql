DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '001- Making TransactionType a lookup table'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
	
	PRINT @MigrationName;

	if not exists(select c.column_id from sys.columns c where c.[name] = 'TransactionTypeDisplayName')
	begin 
		alter table dbo.TransactionType add TransactionTypeDisplayName varchar(50)
	end
	
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Ray Lee', @MigrationName, '001- Making TransactionType a lookup table'
END