INSERT INTO dbo.ParcelLedger(
	[ParcelID], 
	[TransactionDate], 
	[TransactionTypeID], 
	[TransactionAmount], 
	[TransactionDescription]
)

SELECT [ParcelID],
       concat([WaterMonth], '/2/', [WaterYear]) as TransactionDate,
	   TT.TransactionTypeID,
       -[EvapotranspirationRate] as TransactionAmount,
	   concat(WaterMonth, '/', WaterYear, ' Usage from OpenET has been withdrawn from this water account') as TransactionDescription
  FROM [dbo].[ParcelMonthlyEvapotranspiration]
  JOIN dbo.TransactionType TT ON TT.TransactionTypeName = 'Measured Usage'

  INSERT INTO dbo.ParcelLedger(
	[ParcelID], 
	[TransactionDate], 
	[TransactionTypeID], 
	[TransactionAmount], 
	[TransactionDescription]
)

SELECT [ParcelID],
       concat([WaterMonth], '/2/', [WaterYear]) as TransactionDate,
	   TT.TransactionTypeID,
       -[OverriddenEvapotranspirationRate] as TransactionAmount,
	   concat('A correction to ', WaterMonth, '/', WaterYear, ' has been applied to this water account') as TransactionDescription
  FROM [dbo].[ParcelMonthlyEvapotranspiration] pme
  JOIN dbo.TransactionType TT ON TT.TransactionTypeName = 'Measured Usage Correction'
  WHERE pme.OverriddenEvapotranspirationRate is not null