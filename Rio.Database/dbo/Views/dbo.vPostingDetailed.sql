create view dbo.vPostingDetailed
as

	select p.PostingID, p.PostingDate, pt.PostingTypeID, pt.PostingTypeDisplayName
	, ps.PostingStatusID, ps.PostingStatusDisplayName
	, u.UserID as PostedByUserID, u.FirstName as PostedByFirstName, u.LastName as PostedByLastName, u.Email as PostedByEmail
	, p.Price, p.Quantity, p.AvailableQuantity
	, p.CreateAccountID as PostedByAccountID
	, a.AccountName as PostedByAccountName
	, count(t.TradeID) as NumberOfOffers
	from dbo.Posting p
	join dbo.PostingType pt on p.PostingTypeID = pt.PostingTypeID
	join dbo.PostingStatus ps on p.PostingStatusID = ps.PostingStatusID
	left join dbo.Trade t on p.PostingID = t.PostingID
	left join dbo.[User] u on p.CreateUserID = u.UserID
	left join dbo.[Account] a on p.CreateAccountID = a.AccountID
	group by p.PostingID, p.PostingDate, pt.PostingTypeID, pt.PostingTypeDisplayName
	, ps.PostingStatusID, ps.PostingStatusDisplayName
	, u.UserID, u.FirstName, u.LastName, u.Email, p.CreateAccountID, a.AccountName
	, p.Price, p.Quantity, p.AvailableQuantity

GO
/*
select *
from dbo.vPostingDetailed
*/