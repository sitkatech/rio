SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelAllocationHistory](
	[ParcelAllocationHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelAllocationHistoryDate] [datetime] NOT NULL,
	[ParcelAllocationHistoryWaterYear] [int] NOT NULL,
	[ParcelAllocationTypeID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[FileResourceID] [int] NULL,
	[ParcelAllocationHistoryValue] [decimal](10, 4) NULL,
 CONSTRAINT [PK_ParcelAllocationHistory_ParcelAllocationHistoryID] PRIMARY KEY CLUSTERED 
(
	[ParcelAllocationHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ParcelAllocationHistory]  WITH CHECK ADD  CONSTRAINT [FK_ParcelAllocationHistory_FileResource_FileResourceID] FOREIGN KEY([FileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[ParcelAllocationHistory] CHECK CONSTRAINT [FK_ParcelAllocationHistory_FileResource_FileResourceID]
GO
ALTER TABLE [dbo].[ParcelAllocationHistory]  WITH CHECK ADD  CONSTRAINT [FK_ParcelAllocationHistory_ParcelAllocationType_ParcelAllocationTypeID] FOREIGN KEY([ParcelAllocationTypeID])
REFERENCES [dbo].[ParcelAllocationType] ([ParcelAllocationTypeID])
GO
ALTER TABLE [dbo].[ParcelAllocationHistory] CHECK CONSTRAINT [FK_ParcelAllocationHistory_ParcelAllocationType_ParcelAllocationTypeID]
GO
ALTER TABLE [dbo].[ParcelAllocationHistory]  WITH CHECK ADD  CONSTRAINT [FK_ParcelAllocationHistory_User_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ParcelAllocationHistory] CHECK CONSTRAINT [FK_ParcelAllocationHistory_User_UserID]