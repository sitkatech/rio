MERGE INTO dbo.SupportRequestType AS Target
USING (VALUES
(1, 'ReportBug', 'Ran into a bug or problem with this system', 7),
(2, 'ForgotLoginInfo', 'Can''t log in (forgot my username or password, account is locked, etc.)', 2),
(3, 'NewOrganization', 'Need an Organization added to the list', 4),
(4, 'ProvideFeedback', 'Provide Feedback on the site', 6),
(5, 'RequestOrganizationNameChange', 'Request a change to an Organization''s name', 9),
(6, 'Other', 'Other', 100),
(7, 'RequestToChangeUserAccountPrivileges', 'Request to change user account privileges', 10)
)
AS Source (SupportRequestTypeID, SupportRequestTypeName, SupportRequestTypeDisplayName, SupportRequestTypeSortOrder)
ON Target.SupportRequestTypeID = Source.SupportRequestTypeID
WHEN MATCHED THEN
UPDATE SET
	SupportRequestTypeName = Source.SupportRequestTypeName,
	SupportRequestTypeDisplayName = Source.SupportRequestTypeDisplayName,
	SupportRequestTypeSortOrder = Source.SupportRequestTypeSortOrder
WHEN NOT MATCHED BY TARGET THEN
	INSERT (SupportRequestTypeID, SupportRequestTypeName, SupportRequestTypeDisplayName, SupportRequestTypeSortOrder)
	VALUES (SupportRequestTypeID, SupportRequestTypeName, SupportRequestTypeDisplayName, SupportRequestTypeSortOrder)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
