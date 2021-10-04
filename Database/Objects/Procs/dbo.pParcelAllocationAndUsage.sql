IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pParcelAllocationAndUsage'))
    drop procedure dbo.pParcelAllocationAndUsage
go

create procedure dbo.pParcelAllocationAndUsage
(
    @year int
)
as

begin

	select p.ParcelID, p.ParcelNumber, p.ParcelAreaInAcres,
			pal.Allocation, pal.ProjectWater, pal.Reconciliation, pal.NativeYield, pal.StoredWater,
			pal.UsageToDate, a.AccountID, a.AccountName, a.AccountNumber
	from dbo.Parcel p
	join dbo.AccountParcelWaterYear apwy on p.ParcelID = apwy.ParcelID
	join dbo.WaterYear wy on apwy.WaterYearID = wy.WaterYearID and wy.[Year] = @year
	join dbo.[Account] a on apwy.AccountID = a.AccountID
	left join 
	(
		select pl.ParcelID, 
			isnull(sum(case when pl.TransactionTypeID in (11,12,13,14,15,16) then pl.TransactionAmount else 0 end), 0) as Allocation, 
			sum(case when pl.TransactionTypeID = 11 then pl.TransactionAmount else 0 end) as ProjectWater,
			sum(case when pl.TransactionTypeID = 12 then pl.TransactionAmount else 0 end) as Reconciliation,
			sum(case when pl.TransactionTypeID = 13 then pl.TransactionAmount else 0 end) as NativeYield,
			sum(case when pl.TransactionTypeID = 14 then pl.TransactionAmount else 0 end) as StoredWater,
			sum(case when pl.TransactionTypeID in (17,18) then abs(pl.TransactionAmount) else 0 end) as UsageToDate

		from dbo.ParcelLedger pl 
		join dbo.TransactionType tt on pl.TransactionTypeID = tt.TransactionTypeID
		where year(pl.TransactionDate) = @year
		group by pl.ParcelID
	) pal on p.ParcelID = pal.ParcelID

end