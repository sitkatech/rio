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
	[GoogleBucketFileSuffixForRetrieval] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TrackingNumber] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_OpenETSyncHistory_OpenETSyncHistoryID] PRIMARY KEY CLUSTERED 
(
	[OpenETSyncHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
CREATE UNIQUE NONCLUSTERED INDEX [OpenETSyncHistory_GoogleBucketFileSuffixForRetrieval_NotNull] ON [dbo].[OpenETSyncHistory]
(
	[GoogleBucketFileSuffixForRetrieval] ASC
)
WHERE ([GoogleBucketFileSuffixForRetrieval] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
CREATE UNIQUE NONCLUSTERED INDEX [OpenETSyncHistory_TrackingNumber_NotNull] ON [dbo].[OpenETSyncHistory]
(
	[TrackingNumber] ASC
)
WHERE ([TrackingNumber] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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