IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pParcelLedgerBulkCreateParcelReport'))
    drop procedure dbo.pParcelLedgerBulkCreateParcelReport
go

create procedure dbo.pParcelLedgerBulkCreateParcelReport
as

begin

	declare @year int
	set @year = Year(GETUTCDATE())

	select p.ParcelNumber, p.ParcelAreaInAcres, a.AccountID, concat('#', a.AccountNumber, ' (', a.AccountName, ')') as AccountDisplayName, 
			pal.Allocation, pal.ProjectWater, pal.NativeYield, pal.StoredWater, pal.Precipitation
		from dbo.Parcel p
		join dbo.AccountParcelWaterYear apwy on p.ParcelID = apwy.ParcelID
		join dbo.WaterYear wy on apwy.WaterYearID = wy.WaterYearID and wy.[Year] = @year
		join dbo.[Account] a on apwy.AccountID = a.AccountID
		left join (
			select pl.ParcelID,
				isnull(sum(case when pl.TransactionTypeID = 1 and ParcelLedgerEntrySourceTypeID = 1 then pl.TransactionAmount else 0 end), 0) as Allocation, 
				sum(case when pl.TransactionTypeID = 1 and pl.WaterTypeID = 1 then pl.TransactionAmount else 0 end) as ProjectWater,
				sum(case when pl.TransactionTypeID = 1 and pl.WaterTypeID = 3 then pl.TransactionAmount else 0 end) as NativeYield,
				sum(case when pl.TransactionTypeID = 1 and pl.WaterTypeID = 2 then pl.TransactionAmount else 0 end) as Reconciliation,
				sum(case when pl.TransactionTypeID = 1 and pl.WaterTypeID = 4 then pl.TransactionAmount else 0 end) as StoredWater,
				sum(case when pl.TransactionTypeID = 1 and pl.WaterTypeID = 5 then pl.TransactionAmount else 0 end) as Precipitation
			from dbo.ParcelLedger pl
			where year(pl.EffectiveDate) = @year
			group by pl.ParcelID
		) pal on p.ParcelID = pal.ParcelID

end