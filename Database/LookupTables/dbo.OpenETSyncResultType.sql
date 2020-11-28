MERGE INTO dbo.OpenETSyncResultType AS Target
USING (VALUES
(1, 'InProgress', 'In Progress'),
(2, 'Succeeded', 'Succeeded'),
(3, 'Failed', 'Failed')
)
AS Source (OpenETSyncResultTypeID, OpenETSyncResultTypeName, OpenETSyncResultTypeDisplayName)
ON Target.OpenETSyncResultTypeID = Source.OpenETSyncResultTypeID
WHEN MATCHED THEN
UPDATE SET
	OpenETSyncResultTypeName = Source.OpenETSyncResultTypeName,
	OpenETSyncResultTypeDisplayName = Source.OpenETSyncResultTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (OpenETSyncResultTypeID, OpenETSyncResultTypeName, OpenETSyncResultTypeDisplayName)
	VALUES (OpenETSyncResultTypeID, OpenETSyncResultTypeName, OpenETSyncResultTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;