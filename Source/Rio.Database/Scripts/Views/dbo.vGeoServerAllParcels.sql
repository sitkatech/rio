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
                p.ParcelGeometry
                
    FROM        dbo.Parcel p

/*
select * from dbo.vGeoServerAllParcels
*/