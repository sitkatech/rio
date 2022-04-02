CREATE TABLE [dbo].[ParcelUpdateStaging](
	[ParcelUpdateStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelUpdateStaging_ParcelUpdateStagingID] PRIMARY KEY,
	[ParcelNumber] [varchar](20) NOT NULL,
	[ParcelGeometry] [geometry] NULL,
	[OwnerName] [varchar](100) NOT NULL,
	[ParcelGeometry4326] [geometry] NULL,
	[ParcelGeometryText] [varchar](max) NOT NULL,
	[ParcelGeometry4326Text] [varchar](max) NOT NULL,
	[HasConflict] [bit] NOT NULL,
)
