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
                p.ParcelGeometry
                
FROM        dbo.Parcel p

GO
/*
select * from dbo.vGeoServerAllParcels
*/