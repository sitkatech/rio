IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pLandOwnerUsageReport'))
    drop procedure dbo.pLandOwnerUsageReport
go

create procedure dbo.pLandOwnerUsageReport
(
    @year int
)
as

begin

select a.UserID, a.FirstName, a.LastName, a.Email, a.Allocation, a.Purchased, a.Sold,
a.Allocation + a.Purchased - a.Sold as TotalSupply, a.UsageToDate,
a.Allocation + a.Purchased - a.Sold - a.UsageToDate as CurrentAvailable,
a.NumberOfPostings, a.NumberOfTrades,
mrtr.TradeNumber as MostRecentTradeNumber
from
(
	select u.UserID, u.FirstName, u.LastName, u.Email, isnull(pa.Allocation, 0) as Allocation,
			isnull(wts.Purchased, 0) as Purchased,
			isnull(wts.Sold, 0) as Sold,
			isnull(pme.UsageToDate, 0) as UsageToDate,
			isnull(post.NumberOfPostings, 0) as NumberOfPostings,
			isnull(tr.NumberOfTrades, 0) as NumberOfTrades
	from dbo.[User] u
	left join
	(
		select u.UserID, sum(pa.AcreFeetAllocated) as Allocation
		from dbo.[User] u
		join dbo.UserParcel up on u.UserID = up.UserID
		join dbo.Parcel p on up.ParcelID = p.ParcelID
		join dbo.ParcelAllocation pa on p.ParcelID = pa.ParcelID and pa.WaterYear = @year
		group by u.UserID
	) pa on u.UserID = pa.UserID
	left join
	(
		select u.UserID, sum(pme.EvapotranspirationRate) as UsageToDate
		from dbo.[User] u
		join dbo.UserParcel up on u.UserID = up.UserID
		join dbo.Parcel p on up.ParcelID = p.ParcelID
		join dbo.ParcelMonthlyEvapotranspiration pme on p.ParcelID = pme.ParcelID and pme.WaterYear = @year
		group by u.UserID
	) pme on u.UserID = pme.UserID
	left join 
	(
		select u.UserID, 
				sum(case when wtr.WaterTransferTypeID = 1 then wt.AcreFeetTransferred else 0 end) as Purchased,
				sum(case when wtr.WaterTransferTypeID = 2 then wt.AcreFeetTransferred else 0 end) as Sold
		from dbo.[User] u
		join dbo.WaterTransferRegistration wtr on u.UserID = wtr.UserID
		join dbo.WaterTransfer wt on wtr.WaterTransferID = wt.WaterTransferID and year(wt.TransferDate) = @year
		group by u.UserID
	) wts on u.UserID = wts.UserID
	left join 
	(
		select u.UserID, count(post.PostingID) as NumberOfPostings
		from dbo.[User] u
		join dbo.Posting post on u.UserID = post.CreateUserID
		group by u.UserID
	) post on u.UserID = post.UserID
	left join
	(
		select u.UserID, count(post.TradeID) as NumberOfTrades
		from dbo.[User] u
		join dbo.Trade post on u.UserID = post.CreateUserID
		group by u.UserID
	) tr on u.UserID = tr.UserID
) a
left join
(
	select th.TradeID, th.TradeNumber, th.UserID, th.TransactionDate, rank() over(partition by th.UserID order by th.TransactionDate desc, th.TradeNumber desc) as Ranking
	from
	(
		select t.TradeID, t.TradeNumber, t.CreateUserID as UserID, coalesce(p.PostingDate, t.TradeDate, o.OfferDate) as TransactionDate
		from dbo.Offer o
		join dbo.Trade t on o.TradeID = t.TradeID
		join dbo.Posting p on t.PostingID = p.PostingID
		union
		select t.TradeID, t.TradeNumber, p.CreateUserID as UserID, coalesce(p.PostingDate, t.TradeDate, o.OfferDate) as TransactionDate
		from dbo.Offer o
		join dbo.Trade t on o.TradeID = t.TradeID
		join dbo.Posting p on t.PostingID = p.PostingID
		union
		select t.TradeID, t.TradeNumber, wtr.UserID, wtr.StatusDate as TransactionDate
		from dbo.WaterTransferRegistration wtr
		join dbo.WaterTransfer wt on wtr.WaterTransferID = wt.WaterTransferID
		join dbo.Offer o on wt.OfferID = o.OfferID
		join dbo.Trade t on o.TradeID = t.TradeID
		where wtr.StatusDate is not null
	) th
) mrtr on a.UserID = mrtr.UserID and mrtr.Ranking = 1


end