MERGE INTO dbo.AccountStatus AS Target
USING (VALUES
(1, 'Active', 'Active'),
(2, 'Inactive', 'Inactive')
)
AS Source (AccountStatusID, AccountStatusName, AccountStatusDisplayName)
ON Target.AccountStatusID = Source.AccountStatusID
WHEN MATCHED THEN
UPDATE SET
	AccountStatusName = Source.AccountStatusName,
	AccountStatusDisplayName = Source.AccountStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (AccountStatusID, AccountStatusName, AccountStatusDisplayName)
	VALUES (AccountStatusID, AccountStatusName, AccountStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
