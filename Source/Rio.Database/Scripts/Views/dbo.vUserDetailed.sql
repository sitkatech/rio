﻿if exists (select * from dbo.sysobjects where id = object_id('dbo.vUserDetailed'))
	drop view dbo.vUserDetailed
go

create view dbo.vUserDetailed
as

select u.UserID, u.UserGuid, u.FirstName, u.LastName, u.Email, u.LoginName, u.Phone, u.Company, r.RoleID, r.RoleDisplayName,
		cast(sign(isnull(au.UserID, 0)) as bit) as HasActiveTrades, 
		sum(case when wtr.WaterTransferTypeID = 1 then wt.AcreFeetTransferred else 0 end) as AcreFeetOfWaterPurchased,
		sum(case when wtr.WaterTransferTypeID = 2 then wt.AcreFeetTransferred else 0 end) as AcreFeetOfWaterSold
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
left join dbo.WaterTransferRegistration wtr on u.UserID = wtr.UserID
left join dbo.WaterTransfer wt on wtr.WaterTransferID = wt.WaterTransferID
group by u.UserID, u.UserGuid, u.FirstName, u.LastName, u.Email, u.LoginName, u.Phone, u.Company, r.RoleID, r.RoleDisplayName, au.UserID

GO
/*
select *
from dbo.vUserDetailed
*/