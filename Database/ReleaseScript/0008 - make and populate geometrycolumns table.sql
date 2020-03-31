/****** Object:  Table [dbo].[geometry_columns]    Script Date: 3/30/2020 5:59:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[geometry_columns](
	[f_table_catalog] [varchar](128) NOT NULL,
	[f_table_schema] [varchar](128) NOT NULL,
	[f_table_name] [varchar](256) NOT NULL,
	[f_geometry_column] [varchar](256) NOT NULL,
	[coord_dimension] [int] NOT NULL,
	[srid] [int] NOT NULL,
	[geometry_type] [varchar](30) NOT NULL,
 CONSTRAINT [geometry_columns_pk] PRIMARY KEY CLUSTERED 
(
	[f_table_catalog] ASC,
	[f_table_schema] ASC,
	[f_table_name] ASC,
	[f_geometry_column] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[geometry_columns] ([f_table_catalog], [f_table_schema], [f_table_name], [f_geometry_column], [coord_dimension], [srid], [geometry_type]) VALUES (N'RioDB', N'dbo', N'DisadvantagedCommunity', N'ogr_geometry', 2, 4326, N'POLYGON')
INSERT [dbo].[geometry_columns] ([f_table_catalog], [f_table_schema], [f_table_name], [f_geometry_column], [coord_dimension], [srid], [geometry_type]) VALUES (N'RioDB', N'dbo', N'ScenarioWells', N'ogr_geometry', 2, 4326, N'POINT')
