SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[kern_private_wells_clipped](
	[ogr_fid] [int] IDENTITY(1,1) NOT NULL,
	[ogr_geometry] [geometry] NULL,
	[objectid_1] [float] NULL,
	[objectid] [numeric](10, 0) NULL,
	[wcrnumber] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[legacylogn] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[countyname] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[localpermi] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[permitdate] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[permitnumb] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[planneduse] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[recordtype] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[methodofde] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[llaccuracy] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[apn] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[dateworken] [nvarchar](24) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[totaldrill] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[totalcompl] [numeric](10, 0) NULL,
	[topofperfo] [numeric](10, 0) NULL,
	[bottomofpe] [numeric](10, 0) NULL,
	[mtrs] [nvarchar](254) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ddlong] [numeric](24, 15) NULL,
	[ddlat] [numeric](24, 15) NULL,
 CONSTRAINT [PK_kern_private_wells_clipped] PRIMARY KEY CLUSTERED 
(
	[ogr_fid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
CREATE SPATIAL INDEX [ogr_dbo_kern_private_wells_clipped_ogr_geometry_sidx] ON [dbo].[kern_private_wells_clipped]
(
	[ogr_geometry]
)USING  GEOMETRY_GRID 
WITH (BOUNDING_BOX =(-123.6031, 34.1633040000002, -117.35755, 38.8606800000002), GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]