create view dbo.vGeoServerDisadvantagedCommunity
as

select      dc.DisadvantagedCommunityID as PrimaryKey,
			dc.DisadvantagedCommunityName,
			dc.LSADCode,
			dcs.DisadvantagedCommunityStatusName,
			dcs.GeoServerLayerColor,
			dc.DisadvantagedCommunityGeometry
                
FROM        dbo.DisadvantagedCommunity dc
JOIN		dbo.DisadvantagedCommunityStatus dcs on dc.DisadvantagedCommunityStatusID = dcs.DisadvantagedCommunityStatusID

GO
/*
select * from dbo.vGeoServerDisadvantagedCommunity
*/