MERGE INTO dbo.Role AS Target
USING (VALUES
(1, 'Admin', 'Administrator', '', 30),
(2, 'Unassigned', 'Unassigned', '', 10),
(3, 'Normal', 'Normal', '', 20),
(4, 'SitkaAdmin', 'Sitka Admin', '', 40)
)
AS Source (RoleID, RoleName, RoleDisplayName, RoleDescription, SortOrder)
ON Target.RoleID = Source.RoleID
WHEN MATCHED THEN
UPDATE SET
	RoleName = Source.RoleName,
	RoleDisplayName = Source.RoleDisplayName,
	RoleDescription = Source.RoleDescription,
	SortOrder = Source.SortOrder
WHEN NOT MATCHED BY TARGET THEN
	INSERT (RoleID, RoleName, RoleDisplayName, RoleDescription, SortOrder)
	VALUES (RoleID, RoleName, RoleDisplayName, RoleDescription, SortOrder)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
