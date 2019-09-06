MERGE INTO dbo.TradeStatus AS Target
USING (VALUES
(1, 'Countered', 'Countered'),
(2, 'Accepted', 'Accepted'),
(3, 'Rejected', 'Rejected'),
(4, 'Rescinded', 'Rescinded')
)
AS Source (TradeStatusID, TradeStatusName, TradeStatusDisplayName)
ON Target.TradeStatusID = Source.TradeStatusID
WHEN MATCHED THEN
UPDATE SET
	TradeStatusName = Source.TradeStatusName,
	TradeStatusDisplayName = Source.TradeStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (TradeStatusID, TradeStatusName, TradeStatusDisplayName)
	VALUES (TradeStatusID, TradeStatusName, TradeStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
