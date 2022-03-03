MERGE INTO dbo.WaterTransferRegistrationStatus AS Target
USING (VALUES
(1, 'Pending', 'Pending'),
(2, 'Registered', 'Registered'),
(3, 'Canceled', 'Canceled')
)
AS Source (WaterTransferRegistrationStatusID, WaterTransferRegistrationStatusName, WaterTransferRegistrationStatusDisplayName)
ON Target.WaterTransferRegistrationStatusID = Source.WaterTransferRegistrationStatusID
WHEN MATCHED THEN
UPDATE SET
	WaterTransferRegistrationStatusName = Source.WaterTransferRegistrationStatusName,
	WaterTransferRegistrationStatusDisplayName = Source.WaterTransferRegistrationStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WaterTransferRegistrationStatusID, WaterTransferRegistrationStatusName, WaterTransferRegistrationStatusDisplayName)
	VALUES (WaterTransferRegistrationStatusID, WaterTransferRegistrationStatusName, WaterTransferRegistrationStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
