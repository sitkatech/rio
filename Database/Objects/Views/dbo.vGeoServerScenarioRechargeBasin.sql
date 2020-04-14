if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerScenarioRechargeBasin'))
	drop view dbo.vGeoServerScenarioRechargeBasin
go

create view dbo.vGeoServerScenarioRechargeBasin
as

select      rb.ScenarioRechargeBasinID as PrimaryKey,
			rb.ScenarioRechargeBasinName,
			rb.ScenarioRechargeBasinDisplayName,
			rb.ScenarioRechargeBasinGeometry

                
FROM        dbo.ScenarioRechargeBasin rb

GO
