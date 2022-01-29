
INSERT INTO dbo.ParcelLedger(
	[ParcelID], 
	[TransactionDate], 
	[EffectiveDate],
	[TransactionTypeID], 
	[TransactionAmount], 
	[WaterTypeID],
	[TransactionDescription]
)

SELECT [ParcelID],
       dateadd(hour, 8, cast(concat('1/1/', [WaterYear]) as datetime)) as TransactionDate,
	   dateadd(hour, 8, cast(concat('1/1/', [WaterYear]) as datetime)) as EffectiveDate,
	   TT.TransactionTypeID,
       [AcreFeetAllocated] as TransactionAmount,
	   pa.ParcelAllocationTypeID as WaterTypeID,
	   concat('Supply of ', PAT.ParcelAllocationTypeName, ' for ', WaterYear, ' has been deposited into this water account by an administrator') as TransactionDescription
  FROM [dbo].[ParcelAllocation] PA 
  JOIN dbo.ParcelAllocationType PAT ON PA.ParcelAllocationTypeID = PAT.ParcelAllocationTypeID
  JOIN dbo.TransactionType TT ON TT.TransactionTypeName = 'Allocation'
  where PAT.ParcelAllocationTypeID != 5 -- the existing CIMIS precip data is junk so no need to import it
