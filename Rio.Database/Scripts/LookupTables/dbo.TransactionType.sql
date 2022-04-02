MERGE INTO dbo.TransactionType AS Target
USING (VALUES
(1, 'Supply', 'Supply'),
(2, 'Usage', 'Usage')
)
AS Source (TransactionTypeID, TransactionTypeName, TransactionTypeDisplayName)
ON Target.TransactionTypeID = Source.TransactionTypeID
WHEN MATCHED THEN
UPDATE SET
	TransactionTypeName = Source.TransactionTypeName,
	TransactionTypeDisplayName = Source.TransactionTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (TransactionTypeID, TransactionTypeName, TransactionTypeDisplayName)
	VALUES (TransactionTypeID, TransactionTypeName, TransactionTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
