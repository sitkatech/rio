CREATE TABLE [dbo].[WaterTradingScenarioWell](
	[WaterTradingScenarioWellID] [int] NOT NULL CONSTRAINT [PK_WaterTradingScenarioWell_WaterTradingScenarioWellID] PRIMARY KEY,
	[WaterTradingScenarioWellCountyName] [varchar](100) NOT NULL,
	[WaterTradingScenarioWellGeometry] [geometry] NOT NULL
)
