MERGE INTO dbo.ParcelAllocationType AS Target
USING (VALUES
(1, 'Project Water', 'Project Water'),
(2, 'Reconciliation', 'Reconciliation'),
(3, 'Native Yield', 'Native Yield'),
(4, 'Stored Water', 'Stored Water')
)
AS Source (ParcelAllocationTypeID, ParcelAllocationTypeName, ParcelAllocationTypeDisplayName)
ON Target.ParcelAllocationTypeID = Source.ParcelAllocationTypeID
WHEN MATCHED THEN
UPDATE SET
	ParcelAllocationTypeName = Source.ParcelAllocationTypeName,
	ParcelAllocationTypeDisplayName = Source.ParcelAllocationTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ParcelAllocationTypeID, ParcelAllocationTypeName, ParcelAllocationTypeDisplayName)
	VALUES (ParcelAllocationTypeID, ParcelAllocationTypeName, ParcelAllocationTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
