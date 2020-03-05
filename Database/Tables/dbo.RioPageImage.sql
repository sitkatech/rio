SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RioPageImage](
	[RioPageImageID] [int] IDENTITY(1,1) NOT NULL,
	[RioPageID] [int] NOT NULL,
	[FileResourceID] [int] NOT NULL,
 CONSTRAINT [PK_RioPageImage_RioPageImageID] PRIMARY KEY CLUSTERED 
(
	[RioPageImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_RioPageImage_RioPageImageID_FileResourceID] UNIQUE NONCLUSTERED 
(
	[RioPageImageID] ASC,
	[FileResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[RioPageImage]  WITH CHECK ADD  CONSTRAINT [FK_RioPageImage_FileResource_FileResourceID] FOREIGN KEY([FileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[RioPageImage] CHECK CONSTRAINT [FK_RioPageImage_FileResource_FileResourceID]
GO
ALTER TABLE [dbo].[RioPageImage]  WITH CHECK ADD  CONSTRAINT [FK_RioPageImage_RioPage_RioPageID] FOREIGN KEY([RioPageID])
REFERENCES [dbo].[RioPage] ([RioPageID])
GO
ALTER TABLE [dbo].[RioPageImage] CHECK CONSTRAINT [FK_RioPageImage_RioPage_RioPageID]