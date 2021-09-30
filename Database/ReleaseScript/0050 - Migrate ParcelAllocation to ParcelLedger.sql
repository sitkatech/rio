
INSERT INTO dbo.ParcelLedger(
	[ParcelID], 
	[TransactionDate], 
	[TransactionTypeID], 
	[TransactionAmount], 
	[TransactionDescription]
)

SELECT [ParcelID],
       concat('1/1/', [WaterYear]) as TransactionDate,
	   TT.TransactionTypeID,
       [AcreFeetAllocated] as TransactionAmount,
	   concat('Allocation of ', PAT.ParcelAllocationTypeName, ' for ', WaterYear, ' has been deposited into this water account by an administrator') as TransactionDescription
  FROM [dbo].[ParcelAllocation] PA 
  JOIN dbo.ParcelAllocationType PAT ON PA.ParcelAllocationTypeID = PAT.ParcelAllocationTypeID
  JOIN dbo.TransactionType TT ON TT.TransactionTypeName = PAT.ParcelAllocationTypeName 
