# DWRBDO Release Scripts

__For each release script:__
  * Add a line in Script.PreDeployment.ReleaseScripts.sql that calls your release script.
  * Wrap all non idempotent functionality in a conditional that checks the existence of a DatabaseMigration with your ReleaseScript file name.
  * At the end of your release script write an insert statment into DatabaseMigrations to prevent the non idempotent logic to re-run.

## Example Release Script
```sql
DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = 'Pre - 0001 - Example Release Script'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
	
	PRINT @MigrationName;

    /*
        Do work here!
    */
	
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Mikey Knowles', @MigrationName, 'This is an example release script!'
END
```
