SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScenarioWells](
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
 CONSTRAINT [PK_ScenarioWells] PRIMARY KEY CLUSTERED 
(
	[ogr_fid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
