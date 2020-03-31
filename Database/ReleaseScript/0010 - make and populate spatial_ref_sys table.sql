/****** Object:  Table [dbo].[spatial_ref_sys]    Script Date: 3/30/2020 5:59:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[spatial_ref_sys](
	[srid] [int] NOT NULL,
	[auth_name] [varchar](256) NULL,
	[auth_srid] [int] NULL,
	[srtext] [varchar](2048) NULL,
	[proj4text] [varchar](2048) NULL,
PRIMARY KEY CLUSTERED 
(
	[srid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[spatial_ref_sys] ([srid], [auth_name], [auth_srid], [srtext], [proj4text]) VALUES (4326, N'EPSG', 4326, N'GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6326"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AXIS["Latitude",NORTH],AXIS["Longitude",EAST],AUTHORITY["EPSG","4326"]]', N'+proj=longlat +datum=WGS84 +no_defs')
