SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioRechargeBasin](
	[ScenarioRechargeBasinID] [int] NOT NULL,
	[ScenarioRechargeBasinGeometry] [geometry] NOT NULL,
	[ScenarioRechargeBasinName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ScenarioRechargeBasinAcres] [int] NOT NULL,
	[ScenarioRechargeBasinCapacity] [int] NOT NULL,
	[ScenarioRechargeBasinBasinName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ScenarioRechargeBasin_ScenarioRechargeBasinID] PRIMARY KEY CLUSTERED 
(
	[ScenarioRechargeBasinID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
