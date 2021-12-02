IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelMonthlyEvapotranspirationWithETData'))
    drop procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
go

create procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
as

begin
	declare @transactionDate datetime
	set @transactionDate = GETUTCDATE()
	--We only ever change one month at a time
	declare @effectiveDate datetime
	declare @effectiveDateMonth int
	declare @effectiveDateYear int

	select top 1 @effectiveDate = dateadd(day, -1, dateadd(month, 1, cast(concat(WaterMonth, '/1/', WaterYear) as datetime))), @effectiveDateMonth = WaterMonth, @effectiveDateYear = WaterYear
	from dbo.OpenETGoogleBucketResponseEvapotranspirationData

	drop table if exists #npla

	select p.ParcelID, p.ParcelAreaInAcres, @effectiveDate as EffectiveDate, -((et.EvapotranspirationRateInches / 12) * p.ParcelAreaInAcres) as TransactionAmount
	into #npl
	from dbo.Parcel p
	join dbo.OpenETGoogleBucketResponseEvapotranspirationData et
	on p.ParcelNumber = et.ParcelNumber

	-- insert any corrections first - changes in value
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select npl.ParcelID, 18 as TransactionTypeID, @transactionDate as TransactionDate, npl.EffectiveDate, (abs(pl.TransactionAmount) - abs(npl.TransactionAmount)) as TransactionAmount, concat('A correction to ', @effectiveDateMonth, '/', @effectiveDateYear, ' has been applied to this water account') as TransactionDescription
	from #npl npl
	join dbo.ParcelLedger pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate and pl.WaterTypeID is null
	where npl.TransactionAmount != pl.TransactionAmount

	-- insert any corrections first - if they are no longer in the ET dataset we should treat it as a correction
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select pl.ParcelID, 18 as TransactionTypeID, @transactionDate as TransactionDate, pl.EffectiveDate, -pl.TransactionAmount, concat('A correction to ', @effectiveDateMonth, '/', @effectiveDateYear, ' has been applied to this water account') as TransactionDescription
	from dbo.ParcelLedger pl 
	left join #npl npl on pl.ParcelID = npl.ParcelID and pl.EffectiveDate = npl.EffectiveDate
	--OpenET Ledger entries have a null WaterTypeID
	where npl.ParcelID is null and pl.WaterTypeID is null and pl.EffectiveDate = @effectiveDate

	-- then add any new ones
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select npl.ParcelID, 17 as TransactionTypeID, @transactionDate as TransactionDate, npl.EffectiveDate, npl.TransactionAmount, concat(@effectiveDateMonth, '/', @effectiveDateYear, ' Usage from OpenET has been withdrawn from this water account') as TransactionDescription
	from #npl npl
	left join dbo.ParcelLedger pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate and pl.WaterTypeID is null
	where pl.ParcelLedgerID is null

end