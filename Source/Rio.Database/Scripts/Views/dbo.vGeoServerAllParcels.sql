if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerAllParcels'))
	drop view dbo.vGeoServerAllParcels
go

create view dbo.vGeoServerAllParcels
as

select          p.ParcelID as PrimaryKey,
                p.ParcelID,
                p.ParcelNumber,
                p.OwnerName,
                p.OwnerAddress,
                p.OwnerCity,
                p.ParcelAreaInSquareFeet,
                p.ParcelAreaInAcres,
                p.ParcelGeometry,
                u.UserID,
                u.FirstName + ' ' + u.LastName as LandOwnerFullName
                
FROM        dbo.Parcel p
left join   dbo.UserParcel up on p.ParcelID = up.ParcelID
left join   dbo.[User] u on up.UserID = u.UserID

GO
/*
select * from dbo.vGeoServerAllParcels
*/