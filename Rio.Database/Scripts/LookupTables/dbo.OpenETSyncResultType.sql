MERGE INTO dbo.OpenETSyncResultType AS Target
USING (VALUES
(1, 'InProgress', 'In Progress'),
(2, 'Succeeded', 'Succeeded'),
(3, 'Failed', 'Failed'),
(4, 'NoNewData', 'No New Data'),
(5, 'DataNotAvailable', 'Data Not Available'),
(6, 'Created', 'Created')
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