MERGE INTO dbo.FieldDefinition AS Target
USING (VALUES
(1, 'IsPrimaryContactOrganization', 'Is Primary Contact Organization', '<p>The entity with primary responsibility for organizing, planning, and executing implementation activities for a project or program. This is usually the lead implementer.</p>', 1),
(2, 'Organization', 'Organization', '<p>A partner entity that is directly involved with implementation or funding a project.&nbsp;</p>', 1),
(3, 'Password', 'Password', '<p>Password required to log into the RRB Water Trading Platform in order to access and edit project and program information.</p>', 0),
(4, 'PrimaryContact', 'Primary Contact', '<p>An individual at the listed organization responsible for reporting accomplishments and expenditures achieved by the project or program, and who should be contacted when there are questions related to any project associated to the organization.</p>', 1),
(5, 'OrganizationType', 'Organization Type', '<p>A categorization of an organization, e.g. Local, State, Federal or Private.</p>', 1),
(6, 'Username', 'User name', '<p>Password required to log into the system&nbsp;order to access and edit project and program information that is not allowed by public users.</p>', 1),
(7, 'RoleName', 'Role Name', '<p>The name or title describing&nbsp;function or set of permissions that can be assigned to a user.</p>', 1)
)
AS Source (FieldDefinitionID, FieldDefinitionName, FieldDefinitionDisplayName, DefaultDefinition, CanCustomizeLabel)
ON Target.FieldDefinitionID = Source.FieldDefinitionID
WHEN MATCHED THEN
UPDATE SET
	FieldDefinitionName = Source.FieldDefinitionName,
	FieldDefinitionDisplayName = Source.FieldDefinitionDisplayName,
	DefaultDefinition = Source.DefaultDefinition,
	CanCustomizeLabel = Source.CanCustomizeLabel
WHEN NOT MATCHED BY TARGET THEN
	INSERT (FieldDefinitionID, FieldDefinitionName, FieldDefinitionDisplayName, DefaultDefinition, CanCustomizeLabel)
	VALUES (FieldDefinitionID, FieldDefinitionName, FieldDefinitionDisplayName, DefaultDefinition, CanCustomizeLabel)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
