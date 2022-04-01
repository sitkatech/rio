MERGE INTO dbo.ParcelLedgerEntrySourceType AS Target
USING (VALUES
(1, 'Manual', 'Manual'),
(2, 'OpenET', 'OpenET'),
(3, 'Trade', 'Trade')
)
AS Source (ParcelLedgerEntrySourceTypeID, ParcelLedgerEntrySourceTypeName, ParcelLedgerEntrySourceTypeDisplayName)
ON Target.ParcelLedgerEntrySourceTypeID = Source.ParcelLedgerEntrySourceTypeID
WHEN MATCHED THEN
UPDATE SET
	ParcelLedgerEntrySourceTypeName = Source.ParcelLedgerEntrySourceTypeName,
	ParcelLedgerEntrySourceTypeDisplayName = Source.ParcelLedgerEntrySourceTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ParcelLedgerEntrySourceTypeID, ParcelLedgerEntrySourceTypeName, ParcelLedgerEntrySourceTypeDisplayName)
	VALUES (ParcelLedgerEntrySourceTypeID, ParcelLedgerEntrySourceTypeName, ParcelLedgerEntrySourceTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
