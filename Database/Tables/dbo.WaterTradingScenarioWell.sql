SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WaterTradingScenarioWell](
	[WaterTradingScenarioWellID] [int] NOT NULL,
	[WaterTradingScenarioWellCountyName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[WaterTradingScenarioWellGeometry] [geometry] NOT NULL,
 CONSTRAINT [PK_WaterTradingScenarioWell_WaterTradingScenarioWellID] PRIMARY KEY CLUSTERED 
(
	[WaterTradingScenarioWellID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
