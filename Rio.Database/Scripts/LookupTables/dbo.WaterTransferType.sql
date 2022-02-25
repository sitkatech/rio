MERGE INTO dbo.WaterTransferType AS Target
USING (VALUES
(1, 'Buying', 'Buying'),
(2, 'Selling', 'Selling')
)
AS Source (WaterTransferTypeID, WaterTransferTypeName, WaterTransferTypeDisplayName)
ON Target.WaterTransferTypeID = Source.WaterTransferTypeID
WHEN MATCHED THEN
UPDATE SET
	WaterTransferTypeName = Source.WaterTransferTypeName,
	WaterTransferTypeDisplayName = Source.WaterTransferTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WaterTransferTypeID, WaterTransferTypeName, WaterTransferTypeDisplayName)
	VALUES (WaterTransferTypeID, WaterTransferTypeName, WaterTransferTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
