MERGE INTO dbo.ParcelStatus AS Target
USING (VALUES
(1, 'Active', 'Active'),
(2, 'Inactive', 'Inactive')
)
AS Source (ParcelStatusID, ParcelStatusName, ParcelStatusDisplayName)
ON Target.ParcelStatusID = Source.ParcelStatusID
WHEN MATCHED THEN
UPDATE SET
	ParcelStatusName = Source.ParcelStatusName,
	ParcelStatusDisplayName = Source.ParcelStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ParcelStatusID, ParcelStatusName, ParcelStatusDisplayName)
	VALUES (ParcelStatusID, ParcelStatusName, ParcelStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
