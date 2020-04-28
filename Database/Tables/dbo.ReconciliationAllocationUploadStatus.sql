SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReconciliationAllocationUploadStatus](
	[ReconciliationAllocationUploadStatusID] [int] NOT NULL,
	[ReconciliationAllocationUploadStatusName] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReconciliationAllocationUploadStatusDisplayName] [varchar](22) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ReconciliationAllocationUploadStatus_ReconciliationAllocationUploadStatusID] PRIMARY KEY CLUSTERED 
(
	[ReconciliationAllocationUploadStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ReconciliationAllocationUploadStatus_ReconciliationAllocationUploadStatusDisplayName] UNIQUE NONCLUSTERED 
(
	[ReconciliationAllocationUploadStatusDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ReconciliationAllocationUploadStatus_ReconciliationAllocationUploadStatusName] UNIQUE NONCLUSTERED 
(
	[ReconciliationAllocationUploadStatusName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
