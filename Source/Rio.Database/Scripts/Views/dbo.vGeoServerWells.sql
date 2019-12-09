if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerWells'))
	drop view dbo.vGeoServerWells
go

create view dbo.vGeoServerWells
as

select          p.WellID as PrimaryKey,
                p.WellID,
                p.WellName,
                p.WellGeometry
                
FROM        dbo.Well p

GO
/*
select * from dbo.vGeoServerWells
*/