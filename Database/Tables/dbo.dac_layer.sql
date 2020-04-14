SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dac_layer](
	[ogr_fid] [int] IDENTITY(1,1) NOT NULL,
	[ogr_geometry] [geometry] NULL,
	[objectid] [float] NULL,
	[statefp] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[placefp] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[placens] [nvarchar](8) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[geoid] [nvarchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[namelsad] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[lsad] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[classfp] [nvarchar](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[pcicbsa] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[pcinecta] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[mtfcc] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[funcstat] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[aland] [numeric](24, 15) NULL,
	[awater] [numeric](24, 15) NULL,
	[intptlat] [nvarchar](11) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[intptlon] [nvarchar](12) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[pop] [numeric](10, 0) NULL,
	[mhhi] [numeric](10, 0) NULL,
	[white_ct] [numeric](10, 0) NULL,
	[asian_ct] [numeric](10, 0) NULL,
	[afamer_ct] [numeric](10, 0) NULL,
	[hislat_ct] [numeric](10, 0) NULL,
	[nhpi_ct] [numeric](10, 0) NULL,
	[aind_ct] [numeric](10, 0) NULL,
	[other_ct] [numeric](10, 0) NULL,
	[more2_ct] [numeric](10, 0) NULL,
	[white_per] [numeric](24, 15) NULL,
	[asian_per] [numeric](24, 15) NULL,
	[afamer_per] [numeric](24, 15) NULL,
	[hislat_per] [numeric](24, 15) NULL,
	[nhpi_per] [numeric](24, 15) NULL,
	[aind_per] [numeric](24, 15) NULL,
	[other_per] [numeric](24, 15) NULL,
	[more2_per] [numeric](24, 15) NULL,
	[dac_status] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[shape_leng] [numeric](24, 15) NULL,
	[shape_area] [numeric](24, 15) NULL,
 CONSTRAINT [PK_dac_layer] PRIMARY KEY CLUSTERED 
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
CREATE SPATIAL INDEX [ogr_dbo_dac_layer_ogr_geometry_sidx] ON [dbo].[dac_layer]
(
	[ogr_geometry]
)USING  GEOMETRY_GRID 
WITH (BOUNDING_BOX =(-124.269475, 32.5341750000001, -114.229023, 41.993172), GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]