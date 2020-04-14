SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioArsenicContamination](
	[ScenarioArsenicContaminationID] [int] NOT NULL,
	[ScenarioArsenicContaminationWellID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ScenarioArsenicContaminationWellTypeID] [int] NOT NULL,
	[ScenarioArsenicContaminationSourceID] [int] NOT NULL,
	[ScenarioArsenicContaminationGeometry] [geometry] NOT NULL,
	[ScenarioArsenicContaminationContaminationConcentration] [float] NULL,
 CONSTRAINT [PK_ScenarioArsenicContamination_ScenarioArsenicContaminationID] PRIMARY KEY CLUSTERED 
(
	[ScenarioArsenicContaminationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ScenarioArsenicContamination]  WITH CHECK ADD  CONSTRAINT [FK_ScenarioArsenicContamination_ScenarioArsenicContaminationSource_ScenarioArsenicContaminationSourceID] FOREIGN KEY([ScenarioArsenicContaminationSourceID])
REFERENCES [dbo].[ScenarioArsenicContaminationSource] ([ScenarioArsenicContaminationSourceID])
GO
ALTER TABLE [dbo].[ScenarioArsenicContamination] CHECK CONSTRAINT [FK_ScenarioArsenicContamination_ScenarioArsenicContaminationSource_ScenarioArsenicContaminationSourceID]
GO
ALTER TABLE [dbo].[ScenarioArsenicContamination]  WITH CHECK ADD  CONSTRAINT [FK_ScenarioArsenicContamination_ScenarioArsenicContaminationWellType_ScenarioArsenicContaminationWellTypeID] FOREIGN KEY([ScenarioArsenicContaminationWellTypeID])
REFERENCES [dbo].[ScenarioArsenicContaminationWellType] ([ScenarioArsenicContaminationWellTypeID])
GO
ALTER TABLE [dbo].[ScenarioArsenicContamination] CHECK CONSTRAINT [FK_ScenarioArsenicContamination_ScenarioArsenicContaminationWellType_ScenarioArsenicContaminationWellTypeID]