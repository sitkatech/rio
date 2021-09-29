SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpenETSyncHistory](
	[OpenETSyncHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[WaterYearMonthID] [int] NOT NULL,
	[OpenETSyncResultTypeID] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[GoogleBucketFileRetrievalURL] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ErrorMessage] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_OpenETSyncHistory_OpenETSyncHistoryID] PRIMARY KEY CLUSTERED 
(
	[OpenETSyncHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[OpenETSyncHistory]  WITH CHECK ADD  CONSTRAINT [FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID] FOREIGN KEY([OpenETSyncResultTypeID])
REFERENCES [dbo].[OpenETSyncResultType] ([OpenETSyncResultTypeID])
GO
ALTER TABLE [dbo].[OpenETSyncHistory] CHECK CONSTRAINT [FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID]
GO
ALTER TABLE [dbo].[OpenETSyncHistory]  WITH CHECK ADD  CONSTRAINT [FK_OpenETSyncHistory_WaterYearMonth_WaterYearMonthID] FOREIGN KEY([WaterYearMonthID])
REFERENCES [dbo].[WaterYearMonth] ([WaterYearMonthID])
GO
ALTER TABLE [dbo].[OpenETSyncHistory] CHECK CONSTRAINT [FK_OpenETSyncHistory_WaterYearMonth_WaterYearMonthID]