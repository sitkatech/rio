create view dbo.vGeoServerScenarioRechargeBasin
as

select      rb.ScenarioRechargeBasinID as PrimaryKey,
			rb.ScenarioRechargeBasinName,
			rb.ScenarioRechargeBasinDisplayName,
			rb.ScenarioRechargeBasinGeometry

                
FROM        dbo.ScenarioRechargeBasin rb

GO
