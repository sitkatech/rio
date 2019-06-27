DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0001 - Initial Data Population.sql'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	insert into dbo.RioPage(RioPageTypeID)
	select npt.RioPageTypeID
	from dbo.RioPageType npt

	declare @dateToUse datetime
	set @dateToUse = '6/3/2019'

    set identity_insert dbo.[User] on
	insert into dbo.[User](UserID, UserGuid, FirstName, LastName, Email, Phone, RoleID, CreateDate, UpdateDate, LastActivityDate, IsActive, ReceiveSupportEmails, LoginName, Company)
	values
	(1, 'CD3DAB18-4242-4FE9-AB10-874CA43AAEE2', 'Ray', 'Lee', 'ray@sitkatech.com', NULL, 1, @dateToUse, null, @dateToUse, 1, 0, 'ray@sitkatech.com', 'Sitka Technology Group'),
	(2, '61E2A1D0-6A3F-499C-B72C-2160196006F0', 'Liz', 'Christeleit', 'liz.christeleit@sitkatech.com', NULL, 1, @dateToUse, null, @dateToUse, 1, 0, 'liz.christeleit@sitkatech.com', 'Sitka Technology Group'),
	(3, '2F783A30-36E1-4B0C-A1B6-AA4AFE68DDB3', 'John', 'Burns', 'john.burns@sitkatech.com', '(503) 808-1245', 1, @dateToUse, null, @dateToUse, 1, 1, 'john.burns@sitkatech.com', 'Sitka Technology Group'),
	(4, '53654CA4-F369-4F52-858B-0DB7C80075F3', 'Mike', 'Jolliffe', 'mike.jolliffe@sitkatech.com', null, 1, @dateToUse, null, @dateToUse, 1, 1, 'mike.jolliffe@sitkatech.com', 'Sitka Technology Group')

    set identity_insert dbo.[User] off

	INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
	SELECT 'Ray Lee', @MigrationName, 'Initial data population.'
END