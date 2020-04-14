if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerScenarioArsenicContaminationLocation'))
	drop view dbo.vGeoServerScenarioArsenicContaminationLocation
go

create view dbo.vGeoServerScenarioArsenicContaminationLocation
as

select      ac.ScenarioArsenicContaminationLocationID as PrimaryKey,
			ac.ScenarioArsenicContaminationLocationWellName,
			ac.ScenarioArsenicContaminationLocationGeometry
                
FROM        dbo.ScenarioArsenicContaminationLocation ac

GO