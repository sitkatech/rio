DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0001 - Initial Data Population.sql'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	insert into dbo.RioPage(RioPageTypeID)
	select npt.RioPageTypeID
	from dbo.RioPageType npt

	insert into dbo.OrganizationType(OrganizationTypeID, OrganizationTypeName, OrganizationTypeAbbreviation, LegendColor, IsDefaultOrganizationType)
	values
	(1, 'Federal', 'FED', '#1f77b4', 0),
	(2, 'Local', 'LOC', '#aec7e8', 0),
	(3, 'Private', 'PRI', '#ff7f0e', 1),
	(4, 'State', 'ST', '#ffbb78', 0)

	set identity_insert dbo.Organization on
	insert into dbo.Organization(OrganizationID, OrganizationGuid, OrganizationName, OrganizationShortName, OrganizationTypeID, IsActive, OrganizationUrl)
	values
	(1, '6E020A68-7277-41A2-B627-52046A3D5558', 'Sitka Technology Group', 'Sitka', 3, 1, 'http://sitkatech.com')
	set identity_insert dbo.Organization off

	declare @dateToUse datetime
	set @dateToUse = '6/3/2019'

	insert into dbo.[User](UserGuid, FirstName, LastName, Email, Phone, RoleID, CreateDate, UpdateDate, LastActivityDate, IsActive, OrganizationID, ReceiveSupportEmails, LoginName)
	values
	('CD3DAB18-4242-4FE9-AB10-874CA43AAEE2', 'Ray', 'Lee', 'ray@sitkatech.com', NULL, 1, @dateToUse, null, @dateToUse, 1, 1, 0, 'ray@sitkatech.com'),
	('61E2A1D0-6A3F-499C-B72C-2160196006F0', 'Liz', 'Christeleit', 'liz.christeleit@sitkatech.com', NULL, 1, @dateToUse, null, '6/3/2019', 1, 1, 0, 'liz.christeleit@sitkatech.com'),
	('2F783A30-36E1-4B0C-A1B6-AA4AFE68DDB3', 'John', 'Burns', 'john.burns@sitkatech.com', '(503) 808-1245', 1, @dateToUse, null, '6/3/2019', 1, 1, 1, 'john.burns@sitkatech.com')


	INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
	SELECT 'Ray Lee', @MigrationName, 'Initial data population.'
END