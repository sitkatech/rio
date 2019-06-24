MERGE INTO dbo.AuditLogEventType AS Target
USING (VALUES
	(1, 'Added', 'Added'),
	(2, 'Deleted', 'Deleted'),
	(3, 'Modified', 'Modified')
)
AS Source (AuditLogEventTypeID, AuditLogEventTypeName, AuditLogEventTypeDisplayName)
ON Target.AuditLogEventTypeID = Source.AuditLogEventTypeID
WHEN MATCHED THEN
UPDATE SET
	AuditLogEventTypeName = Source.AuditLogEventTypeName,
	AuditLogEventTypeDisplayName = Source.AuditLogEventTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (AuditLogEventTypeID, AuditLogEventTypeName, AuditLogEventTypeDisplayName)
	VALUES (AuditLogEventTypeID, AuditLogEventTypeName, AuditLogEventTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
