create view dbo.vGeoServerWaterTradingScenarioWell
as

select      dw.WaterTradingScenarioWellID as PrimaryKey,
			dw.WaterTradingScenarioWellCountyName,
			dw.WaterTradingScenarioWellGeometry
                
FROM        dbo.WaterTradingScenarioWell dw

GO
/*
select * from dbo.vGeoServerWaterTradingScenarioWell
*/