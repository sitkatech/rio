if exists (select * from dbo.sysobjects where id = object_id('dbo.vUserDetailed'))
	drop view dbo.vUserDetailed
go

create view dbo.vUserDetailed
as

select u.UserID, u.UserGuid, u.FirstName, u.LastName, u.Email, u.LoginName, u.Phone, u.Company, r.RoleID, r.RoleDisplayName,
		cast(sign(isnull(au.UserID, 0)) as bit) as HasActiveTrades, 
		sum(case when u.UserID = wt.ReceivingUserID then wt.AcreFeetTransferred else 0 end) as AcreFeetOfWaterPurchased,
		sum(case when u.UserID = wt.TransferringUserID then wt.AcreFeetTransferred else 0 end) as AcreFeetOfWaterSold
from dbo.[User] u
join dbo.[Role] r on u.RoleID = r.RoleID
left join (
	select p.CreateUserID as UserID
	from dbo.Posting p
	where p.PostingStatusID = 1
	union
	select t.CreateUserID as UserID
	from dbo.Trade t
	where t.TradeStatusID = 1
	union
	select p.CreateUserID as UserID
	from dbo.Offer p
	where p.OfferStatusID = 1
) au on u.UserID = au.UserID
left join dbo.WaterTransfer wt on u.UserID = wt.ReceivingUserID or u.UserID = wt.TransferringUserID 
group by u.UserID, u.UserGuid, u.FirstName, u.LastName, u.Email, u.LoginName, u.Phone, u.Company, r.RoleID, r.RoleDisplayName, au.UserID

GO
/*
select *
from dbo.vUserDetailed
*/