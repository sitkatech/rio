SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioArsenicContaminationWellType](
	[ScenarioArsenicContaminationWellTypeID] [int] NOT NULL,
	[ScenarioArsenicContaminationWellTypeName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ScenarioArsenicContaminationWellType_ScenarioArsenicContaminationWellTypeID] PRIMARY KEY CLUSTERED 
(
	[ScenarioArsenicContaminationWellTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
