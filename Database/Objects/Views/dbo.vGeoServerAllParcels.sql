if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerAllParcels'))
	drop view dbo.vGeoServerAllParcels
go

create view dbo.vGeoServerAllParcels
as

select          p.ParcelID as PrimaryKey,
                p.ParcelID,
                p.ParcelNumber,
                p.ParcelAreaInSquareFeet,
                p.ParcelAreaInAcres,
                p.ParcelGeometry,
                u.UserID,
                u.FirstName + ' ' + u.LastName as LandOwnerFullName
                
FROM        dbo.Parcel p
left join   dbo.AccountParcel up on p.ParcelID = up.ParcelID
left join   dbo.AccountUser au on up.AccountID = au.AccountID
left join dbo.[User] u on u.UserID = au.UserID

GO
/*
select * from dbo.vGeoServerAllParcels
*/