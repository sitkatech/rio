MERGE INTO dbo.OpenETSyncStatusType AS Target
USING (VALUES
(1, 'Nightly', 'Nightly'),
(2, 'Finalized', 'Finalized'),
(3, 'CurrentlyUpdating', 'Currently Updating')
)
AS Source (OpenETSyncStatusTypeID, OpenETSyncStatusTypeName, OpenETSyncStatusTypeDisplayName)
ON Target.OpenETSyncStatusTypeID = Source.OpenETSyncStatusTypeID
WHEN MATCHED THEN
UPDATE SET
	OpenETSyncStatusTypeName = Source.OpenETSyncStatusTypeName,
	OpenETSyncStatusTypeDisplayName = Source.OpenETSyncStatusTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (OpenETSyncStatusTypeID, OpenETSyncStatusTypeName, OpenETSyncStatusTypeDisplayName)
	VALUES (OpenETSyncStatusTypeID, OpenETSyncStatusTypeName, OpenETSyncStatusTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;