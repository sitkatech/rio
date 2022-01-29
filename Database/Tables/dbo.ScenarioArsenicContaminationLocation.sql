SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioArsenicContaminationLocation](
	[ScenarioArsenicContaminationLocationID] [int] IDENTITY(1,1) NOT NULL,
	[ScenarioArsenicContaminationLocationWellName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ScenarioArsenicContaminationLocationGeometry] [geometry] NOT NULL,
 CONSTRAINT [PK_ScenarioArsenicContamination_ScenarioArsenicContaminationID] PRIMARY KEY CLUSTERED 
(
	[ScenarioArsenicContaminationLocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ScenarioArsenicContaminationLocation_ScenarioArsenicContaminationLocationWellName] UNIQUE NONCLUSTERED 
(
	[ScenarioArsenicContaminationLocationWellName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
