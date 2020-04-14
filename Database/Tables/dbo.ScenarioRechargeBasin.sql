SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioRechargeBasin](
	[ScenarioRechargeBasinID] [int] NOT NULL,
	[ScenarioRechargeBasinName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ScenarioRechargeBasinDisplayName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ScenarioRechargeBasinGeometry] [geometry] NOT NULL,
 CONSTRAINT [PK_ScenarioRechargeBasin_ScenarioRechargeBasinID] PRIMARY KEY CLUSTERED 
(
	[ScenarioRechargeBasinID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ScenarioRechargeBasin_ScenarioRechargeBasinName_ScenarioRechargeBasinDisplayName] UNIQUE NONCLUSTERED 
(
	[ScenarioRechargeBasinName] ASC,
	[ScenarioRechargeBasinDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
