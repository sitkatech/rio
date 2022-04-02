CREATE TABLE [dbo].[OpenETSyncHistory](
	[OpenETSyncHistoryID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OpenETSyncHistory_OpenETSyncHistoryID] PRIMARY KEY,
	[WaterYearMonthID] [int] NOT NULL CONSTRAINT [FK_OpenETSyncHistory_WaterYearMonth_WaterYearMonthID] FOREIGN KEY REFERENCES [dbo].[WaterYearMonth] ([WaterYearMonthID]),
	[OpenETSyncResultTypeID] [int] NOT NULL CONSTRAINT [FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID] FOREIGN KEY REFERENCES [dbo].[OpenETSyncResultType] ([OpenETSyncResultTypeID]),
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[GoogleBucketFileRetrievalURL] [varchar](200) NULL,
	[ErrorMessage] [varchar](max) NULL
)