﻿create procedure dbo.pParcelWaterSupplyAndUsage
(
    @year int
)
as

begin

	select p.ParcelID, p.ParcelNumber, p.ParcelAreaInAcres,
			pal.ManualSupply + pal.Purchased - pal.Sold as TotalSupply, 
			pal.Purchased, pal.Sold, pal.UsageToDate, a.AccountID, a.AccountName, a.AccountNumber
	from dbo.Parcel p
	join dbo.AccountParcelWaterYear apwy on p.ParcelID = apwy.ParcelID
	join dbo.WaterYear wy on apwy.WaterYearID = wy.WaterYearID and wy.[Year] = @year
	join dbo.[Account] a on apwy.AccountID = a.AccountID
	left join 
	(
		select pl.ParcelID, 
			isnull(sum(case when pl.TransactionTypeID = 1 and pl.ParcelLedgerEntrySourceTypeID = 1 then pl.TransactionAmount else 0 end), 0) as ManualSupply,
			isnull(sum(case when pl.ParcelLedgerEntrySourceTypeID = 4 and pl.TransactionAmount > 0 then pl.TransactionAmount else 0 end), 0) as Purchased,
			abs(isnull(sum(case when pl.ParcelLedgerEntrySourceTypeID = 4 and pl.TransactionAmount < 0 then pl.TransactionAmount else 0 end), 0)) as Sold,
			abs(sum(case when pl.TransactionTypeID = 2 then pl.TransactionAmount else 0 end)) as UsageToDate
		from dbo.ParcelLedger pl 
		where year(pl.EffectiveDate) = @year
		group by pl.ParcelID
	) pal on p.ParcelID = pal.ParcelID

end