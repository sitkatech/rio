DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0005 - Associating Users with Accounts'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    -- temporary column so we can build the many-many table after adding accounts
    -- Needs to be in a separate batch so the subsequent queries can find the column
    Alter Table dbo.Account
    Add UserID int null

END
GO

DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0005 - Associating Users with Accounts'

Declare @LandownerRoleID int;
set @LandownerRoleID = 2

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    Insert into dbo.Account (AccountName, AccountStatusID, UserID)
    Select 
	    FirstName + ' ' + LastName as AccountName,
	    Case when IsActive = 1 then 1 else 2 end as AccountStatusID,
	    UserID
    from dbo.[User]
    Where RoleID = @LandownerRoleID

    Insert into dbo.AccountUser (AccountID, UserID)
    Select
	    AccountID,
	    UserID
    From dbo.Account

    Alter Table dbo.Account
    Drop Column UserID
	
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Nick Padinha', @MigrationName, 'Create an "account" entity for each user'
END