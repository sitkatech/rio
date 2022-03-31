create view dbo.vGeoServerScenarioArsenicContaminationLocation
as

select      ac.ScenarioArsenicContaminationLocationID as PrimaryKey,
			ac.ScenarioArsenicContaminationLocationWellName,
			ac.ScenarioArsenicContaminationLocationGeometry
                
FROM        dbo.ScenarioArsenicContaminationLocation ac

GO