IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pPublishCimisPrecipitationDataToParcelLedger'))
    drop procedure dbo.pPublishCimisPrecipitationDataToParcelLedger
go

create procedure dbo.pPublishCimisPrecipitationDataToParcelLedger
as

begin
	
	--Currently the job that triggers this procedure only does one month at a time, which is why we can set the effective date globally
	--If this needs to be updated to account for more than one month's worth of data, be sure to test thoroughly

	declare @transactionDate datetime
	set @transactionDate = GETUTCDATE()


	drop table if exists #npl
	drop table if exists #existingParcelLedgerPrecipData

	select p.ParcelID, p.ParcelAreaInAcres, et.DateMeasured as EffectiveDate, Round(p.ParcelAreaInAcres * et.Precipitation / 12.0, 4) as TransactionAmount
	into #npl
	from dbo.Parcel p
	cross join dbo.CimisPrecipitationDatum et

	select ParcelID, EffectiveDate, sum(TransactionAmount) TransactionAmount
	into #existingParcelLedgerPrecipData
	from dbo.ParcelLedger pal
	where TransactionTypeID in (1) and ParcelLedgerEntrySourceTypeID = 3
	group by ParcelID, EffectiveDate

	-- insert any corrections first - changes in value
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, ParcelLedgerEntrySourceTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select npl.ParcelID, 1 as TransactionTypeID, 3 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, npl.EffectiveDate
	, (npl.TransactionAmount - pl.TransactionAmount) as TransactionAmount
	, concat('A correction to ', month(npl.EffectiveDate), '/', day(npl.EffectiveDate), '/', year(npl.EffectiveDate), ' has been applied to this water account') as TransactionDescription
	from #npl npl
	join #existingParcelLedgerPrecipData pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate
	where abs(npl.TransactionAmount - pl.TransactionAmount) > 0.000001

	-- insert any corrections first - if they are no longer in the ET dataset we should treat it as a correction
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, ParcelLedgerEntrySourceTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select pl.ParcelID, 1 as TransactionTypeID, 3 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, pl.EffectiveDate
	, -pl.TransactionAmount
	, concat('A correction to ', month(npl.EffectiveDate), '/', day(npl.EffectiveDate), '/', year(npl.EffectiveDate), ' has been applied to this water account') as TransactionDescription
	from #existingParcelLedgerPrecipData pl 
	left join #npl npl on pl.ParcelID = npl.ParcelID and pl.EffectiveDate = npl.EffectiveDate
	where npl.ParcelID is null

	-- then add any new ones
	insert into dbo.ParcelLedger(ParcelID, TransactionTypeID, ParcelLedgerEntrySourceTypeID, TransactionDate, EffectiveDate, TransactionAmount, TransactionDescription)
	select npl.ParcelID, 1 as TransactionTypeID, 3 as ParcelLedgerEntrySourceTypeID, @transactionDate as TransactionDate, npl.EffectiveDate
	, npl.TransactionAmount
	, concat(month(npl.EffectiveDate), '/', day(npl.EffectiveDate), '/', year(npl.EffectiveDate), ' Supply from CIMIS has been added to this water account') as TransactionDescription
	from #npl npl
	left join #existingParcelLedgerPrecipData pl on npl.ParcelID = pl.ParcelID and npl.EffectiveDate = pl.EffectiveDate
	where pl.ParcelID is null and npl.TransactionAmount > 0

end 
