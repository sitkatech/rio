MERGE INTO dbo.PostingType AS Target
USING (VALUES
(1, 'Buyer', 'Buyer'),
(2, 'Seller', 'Seller')
)
AS Source (PostingTypeID, PostingTypeName, PostingTypeDisplayName)
ON Target.PostingTypeID = Source.PostingTypeID
WHEN MATCHED THEN
UPDATE SET
	PostingTypeName = Source.PostingTypeName,
	PostingTypeDisplayName = Source.PostingTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (PostingTypeID, PostingTypeName, PostingTypeDisplayName)
	VALUES (PostingTypeID, PostingTypeName, PostingTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
