IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelMonthlyEvapotranspirationWithETData'))
    drop procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
go

create procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
as

begin
	declare @transactionDate datetime
	set @transactionDate = GETUTCDATE()

	select p.ParcelID, p.ParcelAreaInAcres, dateadd(day, -1, dateadd(month, 1, cast(concat(et.WaterMonth, '/1/', et.WaterYear) as datetime))) as EffectiveDate, -((et.EvapotranspirationRateInches / 12) * p.ParcelAreaInAcres) as TransactionAmount
	into #npl
	from dbo.Parcel p
	join dbo.OpenETGoogleBucketResponseEvapotranspirationData et
	on p.ParcelNumber = et.ParcelNumber

	-- TransactionTypeID = 18 => TransactionTypeID = 2, ParcelLedgerEntrySourceTypeID = 2

	-- insert any corrections first - changes in value
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, TransactionDate, EffectiveDate, TransactionAmount)
	select npl.ParcelID, 2 as TransactionTypeID, 2 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, npl.EffectiveDate, (abs(pl.TransactionAmount) - abs(npl.TransactionAmount)) as TransactionAmount
	from #npl npl
	join dbo.ParcelLedger pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate
	where pl.ParcelLedgerID is null	and npl.TransactionAmount != pl.TransactionAmount

	-- insert any corrections first - if they are no longer in the ET dataset we should treat it as a correction
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, TransactionDate, EffectiveDate, TransactionAmount)
	select pl.ParcelID, 2 as TransactionTypeID, 2 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, pl.EffectiveDate, -pl.TransactionAmount
	from dbo.ParcelLedger pl 
	left join #npl npl on pl.ParcelID = npl.ParcelID and pl.EffectiveDate = npl.EffectiveDate
	where npl.ParcelID is null

	-- then add any new ones
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, TransactionDate, EffectiveDate, TransactionAmount)
	select npl.ParcelID, 2 as TransactionTypeID, 2 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, npl.EffectiveDate, npl.TransactionAmount
	from #npl npl
	left join dbo.ParcelLedger pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate
	where pl.ParcelLedgerID is null


end