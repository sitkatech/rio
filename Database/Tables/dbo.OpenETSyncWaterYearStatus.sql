SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpenETSyncWaterYearStatus](
	[OpenETSyncWaterYearStatusID] [int] IDENTITY(1,1) NOT NULL,
	[WaterYear] [int] NOT NULL,
	[OpenETSyncStatusTypeID] [int] NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_OpenETSyncWaterYearStatus_OpenETSyncWaterYearStatusID] PRIMARY KEY CLUSTERED 
(
	[OpenETSyncWaterYearStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_OpenETSyncWaterYearStatus_WaterYear] UNIQUE NONCLUSTERED 
(
	[WaterYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[OpenETSyncWaterYearStatus]  WITH CHECK ADD  CONSTRAINT [FK_OpenETSyncWaterYearStatus_OpenETSyncStatusType_OpenETSyncStatusTypeID] FOREIGN KEY([OpenETSyncStatusTypeID])
REFERENCES [dbo].[OpenETSyncStatusType] ([OpenETSyncStatusTypeID])
GO
ALTER TABLE [dbo].[OpenETSyncWaterYearStatus] CHECK CONSTRAINT [FK_OpenETSyncWaterYearStatus_OpenETSyncStatusType_OpenETSyncStatusTypeID]