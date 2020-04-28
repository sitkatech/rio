SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReconciliationAllocationUpload](
	[ReconciliationAllocationUploadID] [int] IDENTITY(1,1) NOT NULL,
	[UploadUserID] [int] NOT NULL,
	[FileResourceID] [int] NOT NULL,
	[ReconciliationAllocationUploadStatusID] [int] NOT NULL,
 CONSTRAINT [PK_ReconciliationAllocationUpload_ReconciliationAllocationUploadID] PRIMARY KEY CLUSTERED 
(
	[ReconciliationAllocationUploadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ReconciliationAllocationUpload]  WITH CHECK ADD  CONSTRAINT [FK_ReconciliationAllocationUpload_FileResource_FileResourceID] FOREIGN KEY([FileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[ReconciliationAllocationUpload] CHECK CONSTRAINT [FK_ReconciliationAllocationUpload_FileResource_FileResourceID]
GO
ALTER TABLE [dbo].[ReconciliationAllocationUpload]  WITH CHECK ADD  CONSTRAINT [FK_ReconciliationAllocationUpload_ReconciliationAllocationUploadStatus_ReconciliationAllocationUploadStatusID] FOREIGN KEY([ReconciliationAllocationUploadStatusID])
REFERENCES [dbo].[ReconciliationAllocationUploadStatus] ([ReconciliationAllocationUploadStatusID])
GO
ALTER TABLE [dbo].[ReconciliationAllocationUpload] CHECK CONSTRAINT [FK_ReconciliationAllocationUpload_ReconciliationAllocationUploadStatus_ReconciliationAllocationUploadStatusID]
GO
ALTER TABLE [dbo].[ReconciliationAllocationUpload]  WITH CHECK ADD  CONSTRAINT [FK_ReconciliationAllocationUpload_User_UploadUserID_UserID] FOREIGN KEY([UploadUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReconciliationAllocationUpload] CHECK CONSTRAINT [FK_ReconciliationAllocationUpload_User_UploadUserID_UserID]