create procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
as

begin
	--Currently the job that triggers this procedure only does one month at a time, which is why we can set the effective date globally
	--If this needs to be updated to account for more than one month's worth of data, be sure to test thoroughly

	declare @transactionDate datetime
	set @transactionDate = GETUTCDATE()

	declare @effectiveDate datetime
	declare @effectiveDateMonth int
	declare @effectiveDateYear int

	select top 1 @effectiveDate = dateadd(hour, 8,(dateadd(day, -1, dateadd(month, 1, cast(concat(WaterMonth, '/1/', WaterYear) as datetime))))), @effectiveDateMonth = WaterMonth, @effectiveDateYear = WaterYear
	from dbo.OpenETGoogleBucketResponseEvapotranspirationData

	drop table if exists #npl
	drop table if exists #openETPLEntriesForDate

	select p.ParcelID, p.ParcelAreaInAcres, @effectiveDate as EffectiveDate, -((et.EvapotranspirationRateInches / 12) * p.ParcelAreaInAcres) as TransactionAmount
	into #npl
	from dbo.Parcel p
	join dbo.OpenETGoogleBucketResponseEvapotranspirationData et
	on p.ParcelNumber = et.ParcelNumber

	select ParcelID, EffectiveDate, sum(TransactionAmount) TransactionAmount
	into #openETPLEntriesForDate
	from dbo.ParcelLedger pal
	where EffectiveDate = @effectiveDate and TransactionTypeID in (2)
	group by ParcelID, EffectiveDate

	-- insert any corrections first - changes in value
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, ParcelLedgerEntrySourceTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select npl.ParcelID, 2 as TransactionTypeID, 2 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, npl.EffectiveDate, (abs(pl.TransactionAmount) - abs(npl.TransactionAmount)) as TransactionAmount, concat('A correction to ', @effectiveDateMonth, '/', @effectiveDateYear, ' has been applied to this water account') as TransactionDescription
	from #npl npl
	join #openETPLEntriesForDate pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate
	where npl.TransactionAmount != pl.TransactionAmount

	-- insert any corrections first - if they are no longer in the ET dataset we should treat it as a correction
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, ParcelLedgerEntrySourceTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select pl.ParcelID, 2 as TransactionTypeID, 2 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, pl.EffectiveDate, -pl.TransactionAmount, concat('A correction to ', @effectiveDateMonth, '/', @effectiveDateYear, ' has been applied to this water account') as TransactionDescription
	from #openETPLEntriesForDate pl 
	left join #npl npl on pl.ParcelID = npl.ParcelID and pl.EffectiveDate = npl.EffectiveDate
	where npl.ParcelID is null

	-- then add any new ones
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, ParcelLedgerEntrySourceTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select npl.ParcelID, 2 as TransactionTypeID, 2 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, npl.EffectiveDate, npl.TransactionAmount, concat(@effectiveDateMonth, '/', @effectiveDateYear, ' Usage from OpenET has been withdrawn from this water account') as TransactionDescription
	from #npl npl
	left join #openETPLEntriesForDate pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate
	where pl.ParcelID is null

end