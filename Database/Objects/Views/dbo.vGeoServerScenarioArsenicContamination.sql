if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerScenarioArsenicContamination'))
	drop view dbo.vGeoServerScenarioArsenicContamination
go

create view dbo.vGeoServerScenarioArsenicContamination
as

select      ac.ScenarioArsenicContaminationID as PrimaryKey,
			ac.ScenarioArsenicContaminationWellID,
			acwt.ScenarioArsenicContaminationWellTypeName,
			acs.ScenarioArsenicContaminationSourceName,
			ac.ScenarioArsenicContaminationGeometry,
			ac.ScenarioArsenicContaminationContaminationConcentration
			

                
FROM        dbo.ScenarioArsenicContamination ac
JOIN		dbo.ScenarioArsenicContaminationWellType acwt on ac.ScenarioArsenicContaminationWellTypeID = ac.ScenarioArsenicContaminationWellTypeID
JOIN		dbo.ScenarioArsenicContaminationSource acs on ac.ScenarioArsenicContaminationSourceID = acs.ScenarioArsenicContaminationSourceID

GO
/*
select * from dbo.vGeoServerWaterTradingScenarioWell
*/