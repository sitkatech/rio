SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpenETSyncHistory](
	[OpenETSyncHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[OpenETSyncResultTypeID] [int] NOT NULL,
	[WaterYearID] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_OpenETSyncHistory_OpenETSyncHistoryID] PRIMARY KEY CLUSTERED 
(
	[OpenETSyncHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[OpenETSyncHistory]  WITH CHECK ADD  CONSTRAINT [FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID] FOREIGN KEY([OpenETSyncResultTypeID])
REFERENCES [dbo].[OpenETSyncResultType] ([OpenETSyncResultTypeID])
GO
ALTER TABLE [dbo].[OpenETSyncHistory] CHECK CONSTRAINT [FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID]
GO
ALTER TABLE [dbo].[OpenETSyncHistory]  WITH CHECK ADD  CONSTRAINT [FK_OpenETSyncHistory_WaterYear_WaterYearID] FOREIGN KEY([WaterYearID])
REFERENCES [dbo].[WaterYear] ([WaterYearID])
GO
ALTER TABLE [dbo].[OpenETSyncHistory] CHECK CONSTRAINT [FK_OpenETSyncHistory_WaterYear_WaterYearID]