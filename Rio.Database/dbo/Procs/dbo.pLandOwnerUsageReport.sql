﻿create procedure dbo.pLandOwnerUsageReport
(
    @year int
)
as

begin
select a.AccountID, a.AccountName, a.AccountNumber, a.AcresManaged, a.Precipitation, a.Purchased, a.Sold,
a.ManualSupply + a.Precipitation + a.Purchased - a.Sold as TotalSupply, a.UsageToDate,
a.ManualSupply + a.Precipitation + a.Purchased - a.Sold - a.UsageToDate as CurrentAvailable,
wy.OverconsumptionRate, a.OverconsumptionAmount, a.OverconsumptionCharge, a.NumberOfPostings, a.NumberOfTrades,
mrtr.TradeNumber as MostRecentTradeNumber
from
(
	select acc.AccountID, acc.AccountName, acc.AccountNumber, 
			isnull(am.AcresManaged, 0) as AcresManaged,
			isnull(pa.ManualSupply, 0) as ManualSupply,
			isnull(pa.Precipitation, 0) as Precipitation,
			isnull(pa.Purchased, 0) as Purchased,
			isnull(pa.Sold, 0) as Sold,
			isnull(pa.UsageToDate, 0) as UsageToDate,
			isnull(aoc.OverconsumptionAmount, 0) as OverconsumptionAmount,
			isnull(aoc.OverconsumptionCharge, 0) as OverconsumptionCharge,
			isnull(post.NumberOfPostings, 0) as NumberOfPostings,
			isnull(tr.NumberOfTrades, 0) as NumberOfTrades
	from dbo.Account acc
	left join (
		select acc.AccountID, sum(p.ParcelAreaInAcres) as AcresManaged
		from dbo.Account acc
		join (
			select apwy.AccountID, apwy.ParcelID
			from dbo.AccountParcelWaterYear apwy
			join dbo.WaterYear wy on wy.WaterYearID = apwy.WaterYearID
			where wy.[Year] = @year
		) up on acc.AccountID = up.AccountID
		join dbo.Parcel p on up.ParcelID = p.ParcelID
		group by acc.AccountID
	) am on acc.AccountID = am.AccountID
	left join
	(
		select acc.AccountID,
				sum(case when pa.TransactionTypeID = 1 and pa.ParcelLedgerEntrySourceTypeID = 1 then pa.TransactionAmount else 0 end) as ManualSupply, 
				sum(case when pa.ParcelLedgerEntrySourceTypeID = 3 then pa.TransactionAmount else 0 end) as Precipitation,
				sum(case when pa.ParcelLedgerEntrySourceTypeID = 4 and pa.TransactionAmount > 0 then pa.TransactionAmount else 0 end) as Purchased,
				abs(sum(case when pa.ParcelLedgerEntrySourceTypeID = 4 and pa.TransactionAmount < 0 then pa.TransactionAmount else 0 end)) as Sold,
				abs(sum(case when pa.TransactionTypeID = 2 then pa.TransactionAmount else 0 end)) as UsageToDate
		from dbo.Account acc
		join (
			select apwy.AccountID, apwy.ParcelID
			from dbo.AccountParcelWaterYear apwy
			join dbo.WaterYear wy on wy.WaterYearID = apwy.WaterYearID
			where wy.[Year] = @year
		) up on acc.AccountID = up.AccountID
		join dbo.Parcel p on up.ParcelID = p.ParcelID
		join dbo.ParcelLedger pa on p.ParcelID = pa.ParcelID and year(pa.EffectiveDate) = @year
		group by acc.AccountID
	) pa on acc.AccountID = pa.AccountID
	left join
	(
		select aoc.AccountID, aoc.OverconsumptionAmount, aoc.OverconsumptionCharge, wy.OverconsumptionRate
		from dbo.AccountOverconsumptionCharge aoc
		join dbo.WaterYear wy on aoc.WaterYearID = wy.WaterYearID
		where wy.[Year] = @year
	) aoc on acc.AccountID = aoc.AccountID
	left join 
	(
		select acc.AccountID, count(post.PostingID) as NumberOfPostings
		from dbo.Account acc
		join dbo.Posting post on acc.AccountID = post.CreateAccountID
		where year(post.PostingDate) = @year
		group by acc.AccountID
	) post on acc.AccountID = post.AccountID
	left join
	(
		select acc.AccountID, count(post.TradeID) as NumberOfTrades
		from dbo.Account acc
		join dbo.Trade post on acc.AccountID = post.CreateAccountID
		where year(post.TradeDate) = @year
		group by acc.AccountID
	) tr on acc.AccountID = tr.AccountID
) a 
left join
(
	select th.TradeID, th.TradeNumber, th.AccountID, th.TransactionDate, rank() over(partition by th.AccountID order by th.TransactionDate desc, th.TradeNumber desc) as Ranking
	from
	(
		select t.TradeID, t.TradeNumber, t.CreateAccountID as AccountID, coalesce(p.PostingDate, t.TradeDate, o.OfferDate) as TransactionDate
		from dbo.Offer o
		join dbo.Trade t on o.TradeID = t.TradeID
		join dbo.Posting p on t.PostingID = p.PostingID
		union
		select t.TradeID, t.TradeNumber, p.CreateAccountID as AccountID, coalesce(p.PostingDate, t.TradeDate, o.OfferDate) as TransactionDate
		from dbo.Offer o
		join dbo.Trade t on o.TradeID = t.TradeID
		join dbo.Posting p on t.PostingID = p.PostingID
		union
		select t.TradeID, t.TradeNumber, wtr.AccountID, wtr.StatusDate as TransactionDate
		from dbo.WaterTransferRegistration wtr
		join dbo.WaterTransfer wt on wtr.WaterTransferID = wt.WaterTransferID
		join dbo.Offer o on wt.OfferID = o.OfferID
		join dbo.Trade t on o.TradeID = t.TradeID
		where wtr.StatusDate is not null
	) th
) mrtr on a.AccountID = mrtr.AccountID and mrtr.Ranking = 1
left join dbo.WaterYear wy on wy.[Year] = @year

end