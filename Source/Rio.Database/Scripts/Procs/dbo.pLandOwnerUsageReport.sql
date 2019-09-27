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
a.TradeNumber as MostRecentTradeNumber
from
(
	select u.UserID, u.FirstName, u.LastName, u.Email, sum(pa.AcreFeetAllocated) as Allocation,
			sum(case when wtr.WaterTransferTypeID = 1 then wt.AcreFeetTransferred else 0 end) as Purchased,
			sum(case when wtr.WaterTransferTypeID = 2 then wt.AcreFeetTransferred else 0 end) as Sold,
			sum(pme.EvapotranspirationRate) as UsageToDate,
			count(distinct post.PostingID) as NumberOfPostings,
			count(distinct tr.TradeID) as NumberOfTrades,
			mrtr.TradeNumber
	from dbo.[User] u
	left join dbo.UserParcel up on u.UserID = up.UserID
	left join dbo.Parcel p on up.ParcelID = p.ParcelID
	left join dbo.ParcelAllocation pa on p.ParcelID = pa.ParcelID and pa.WaterYear = @year
	left join dbo.ParcelMonthlyEvapotranspiration pme on p.ParcelID = pme.ParcelID and pme.WaterYear = @year
	left join dbo.WaterTransferRegistration wtr on u.UserID = wtr.UserID
	left join dbo.WaterTransfer wt on wtr.WaterTransferID = wt.WaterTransferID and year(wt.TransferDate) = @year
	left join dbo.Posting post on u.UserID = post.CreateUserID
	left join dbo.Trade tr on u.UserID = tr.CreateUserID
	left join
	(
		select th.TradeID, th.TradeNumber, th.UserID, th.TransactionDate, rank() over(partition by th.UserID order by th.TransactionDate desc) as Ranking
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
			select t.TradeID, t.TradeNumber, wtr.UserID, wtr.DateRegistered as TransactionDate
			from dbo.WaterTransferRegistration wtr
			join dbo.WaterTransfer wt on wtr.WaterTransferID = wt.WaterTransferID
			join dbo.Offer o on wt.OfferID = o.OfferID
			join dbo.Trade t on o.TradeID = t.TradeID
			where wtr.DateRegistered is not null
		) th
	) mrtr on u.UserID = mrtr.UserID and mrtr.Ranking = 1
	group by u.UserID, u.FirstName, u.LastName, u.Email, mrtr.TradeNumber
) a

end