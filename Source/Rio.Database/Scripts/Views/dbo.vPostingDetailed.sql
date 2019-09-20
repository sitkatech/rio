if exists (select * from dbo.sysobjects where id = object_id('dbo.vPostingDetailed'))
	drop view dbo.vPostingDetailed
go

create view dbo.vPostingDetailed
as

	select p.PostingID, p.PostingDate, pt.PostingTypeID, pt.PostingTypeDisplayName
	, ps.PostingStatusID, ps.PostingStatusDisplayName
	, u.UserID as PostedByUserID, u.FirstName as PostedByFirstName, u.LastName as PostedByLastName, u.Email as PostedByEmail
	, p.Price, p.Quantity, p.AvailableQuantity
	, count(t.TradeID) as NumberOfOffers
	from dbo.Posting p
	join dbo.PostingType pt on p.PostingTypeID = pt.PostingTypeID
	join dbo.PostingStatus ps on p.PostingStatusID = ps.PostingStatusID
	left join dbo.Trade t on p.PostingID = t.PostingID
	left join dbo.[User] u on p.CreateUserID = u.UserID
	group by p.PostingID, p.PostingDate, pt.PostingTypeID, pt.PostingTypeDisplayName
	, ps.PostingStatusID, ps.PostingStatusDisplayName
	, u.UserID, u.FirstName, u.LastName, u.Email
	, p.Price, p.Quantity, p.AvailableQuantity

GO
/*
select *
from dbo.vPostingDetailed
*/