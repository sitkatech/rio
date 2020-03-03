MERGE INTO dbo.PostingStatus AS Target
USING (VALUES
(1, 'Open', 'Open'),
(2, 'Closed', 'Closed')
)
AS Source (PostingStatusID, PostingStatusName, PostingStatusDisplayName)
ON Target.PostingStatusID = Source.PostingStatusID
WHEN MATCHED THEN
UPDATE SET
	PostingStatusName = Source.PostingStatusName,
	PostingStatusDisplayName = Source.PostingStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (PostingStatusID, PostingStatusName, PostingStatusDisplayName)
	VALUES (PostingStatusID, PostingStatusName, PostingStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
