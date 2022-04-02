CREATE TABLE [dbo].[ScenarioRechargeBasin](
	[ScenarioRechargeBasinID] [int] NOT NULL CONSTRAINT [PK_ScenarioRechargeBasin_ScenarioRechargeBasinID] PRIMARY KEY,
	[ScenarioRechargeBasinName] [varchar](100) NOT NULL,
	[ScenarioRechargeBasinDisplayName] [varchar](100) NOT NULL,
	[ScenarioRechargeBasinGeometry] [geometry] NOT NULL,
	CONSTRAINT [AK_ScenarioRechargeBasin_ScenarioRechargeBasinName_ScenarioRechargeBasinDisplayName] UNIQUE ([ScenarioRechargeBasinName], [ScenarioRechargeBasinDisplayName])
)
