SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelUpdateStaging](
	[ParcelUpdateStagingID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelNumber] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParcelGeometry] [geometry] NULL,
	[OwnerName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParcelGeometry4326] [geometry] NULL,
	[ParcelGeometryText] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParcelGeometry4326Text] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[HasConflict] [bit] NOT NULL,
 CONSTRAINT [PK_ParcelUpdateStaging_ParcelUpdateStagingID] PRIMARY KEY CLUSTERED 
(
	[ParcelUpdateStagingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
