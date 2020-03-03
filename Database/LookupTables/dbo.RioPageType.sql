MERGE INTO dbo.RioPageType AS Target
USING (VALUES
(1, 'HomePage', 'Home Page'),
(2, 'About', 'About'),
(3, 'OrganizationsList', 'Organizations List'),
(4, 'UsersList', 'Users List')
)
AS Source (RioPageTypeID, RioPageTypeName, RioPageTypeDisplayName)
ON Target.RioPageTypeID = Source.RioPageTypeID
WHEN MATCHED THEN
UPDATE SET
	RioPageTypeName = Source.RioPageTypeName,
	RioPageTypeDisplayName = Source.RioPageTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (RioPageTypeID, RioPageTypeName, RioPageTypeDisplayName)
	VALUES (RioPageTypeID, RioPageTypeName, RioPageTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
