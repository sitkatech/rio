if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerScenarioRechargeBasin'))
	drop view dbo.vGeoServerScenarioRechargeBasin
go

create view dbo.vGeoServerScenarioRechargeBasin
as

select      rb.ScenarioRechargeBasinID as PrimaryKey,
			rb.ScenarioRechargeBasinGeometry,
			rb.ScenarioRechargeBasinName,
			rb.ScenarioRechargeBasinAcres,
			rb.ScenarioRechargeBasinCapacity,
			rb.ScenarioRechargeBasinBasinName

                
FROM        dbo.ScenarioRechargeBasin rb

GO
