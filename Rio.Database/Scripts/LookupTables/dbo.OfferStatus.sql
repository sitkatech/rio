MERGE INTO dbo.OfferStatus AS Target
USING (VALUES
(1, 'Pending', 'Pending'),
(2, 'Rejected', 'Rejected'),
(3, 'Rescinded', 'Rescinded'),
(4, 'Accepted', 'Accepted')
)
AS Source (OfferStatusID, OfferStatusName, OfferStatusDisplayName)
ON Target.OfferStatusID = Source.OfferStatusID
WHEN MATCHED THEN
UPDATE SET
	OfferStatusName = Source.OfferStatusName,
	OfferStatusDisplayName = Source.OfferStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (OfferStatusID, OfferStatusName, OfferStatusDisplayName)
	VALUES (OfferStatusID, OfferStatusName, OfferStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
