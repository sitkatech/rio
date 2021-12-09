INSERT INTO dbo.ParcelLedger(
	ParcelID, 
	TransactionDate, 
	EffectiveDate,
	TransactionTypeID, 
	TransactionAmount, 
	TransactionDescription
)

SELECT ParcelID,
	   dateadd(hour, 8, dateadd(day, -1, dateadd(month, 1, cast(concat(WaterMonth, '/1/', WaterYear) as datetime)))) as TransactionDate,
	   dateadd(hour, 8, dateadd(day, -1, dateadd(month, 1, cast(concat(WaterMonth, '/1/', WaterYear) as datetime)))) as EffectiveDate,
	   TT.TransactionTypeID,
       -EvapotranspirationRate as TransactionAmount,
	   concat(WaterMonth, '/', WaterYear, ' Usage from OpenET has been withdrawn from this water account') as TransactionDescription
  FROM dbo.ParcelMonthlyEvapotranspiration
  JOIN dbo.TransactionType TT ON TT.TransactionTypeName = 'Measured Usage'

  INSERT INTO dbo.ParcelLedger(
	ParcelID, 
	TransactionDate, 
	EffectiveDate,
	TransactionTypeID, 
	TransactionAmount, 
	TransactionDescription
)

SELECT ParcelID,
	   dateadd(second, 1, dateadd(hour, 8, dateadd(day, -1, dateadd(month, 1, cast(concat(WaterMonth, '/1/', WaterYear) as datetime))))) as TransactionDate,
	   dateadd(hour, 8, dateadd(day, -1, dateadd(month, 1, cast(concat(WaterMonth, '/1/', WaterYear) as datetime)))) as EffectiveDate,
	   TT.TransactionTypeID,
       -OverriddenEvapotranspirationRate as TransactionAmount,
	   concat('A correction to ', WaterMonth, '/', WaterYear, ' has been applied to this water account') as TransactionDescription
  FROM dbo.ParcelMonthlyEvapotranspiration pme
  JOIN dbo.TransactionType TT ON TT.TransactionTypeName = 'Measured Usage Correction'
  WHERE pme.OverriddenEvapotranspirationRate is not null