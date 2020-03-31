SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DisadvantagedCommunity](
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
 CONSTRAINT [PK_DisadvantagedCommunity] PRIMARY KEY CLUSTERED 
(
	[ogr_fid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
