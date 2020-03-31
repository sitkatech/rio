if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerDisadvantagedCommunity'))
	drop view dbo.vGeoServerDisadvantagedCommunity
go

create view dbo.vGeoServerDisadvantagedCommunity
as

select      dc.DisadvantagedCommunityID as PrimaryKey,
			dc.DisadvantagedCommunityName,
			dc.LSADCode,
			dcs.DisadvantagedCommunityStatusName,
			dcs.GeoServerLayerColor   
                
FROM        dbo.DisadvantagedCommunity dc
JOIN		dbo.DisadvantagedCommunityStatus dcs on dc.DisadvantagedCommunityStatusID = dcs.DisadvantagedCommunityStatusID

GO
/*
select * from dbo.vGeoServerDisadvantagedCommunity
*/