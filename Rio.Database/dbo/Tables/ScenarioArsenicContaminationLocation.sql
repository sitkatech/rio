CREATE TABLE [dbo].[ScenarioArsenicContaminationLocation](
	[ScenarioArsenicContaminationLocationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ScenarioArsenicContamination_ScenarioArsenicContaminationID] PRIMARY KEY,
	[ScenarioArsenicContaminationLocationWellName] [varchar](50) NOT NULL CONSTRAINT [AK_ScenarioArsenicContaminationLocation_ScenarioArsenicContaminationLocationWellName] UNIQUE,
	[ScenarioArsenicContaminationLocationGeometry] [geometry] NOT NULL
)
