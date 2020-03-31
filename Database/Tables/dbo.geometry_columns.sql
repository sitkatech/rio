SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[geometry_columns](
	[f_table_catalog] [varchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[f_table_schema] [varchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[f_table_name] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[f_geometry_column] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[coord_dimension] [int] NOT NULL,
	[srid] [int] NOT NULL,
	[geometry_type] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [geometry_columns_pk] PRIMARY KEY CLUSTERED 
(
	[f_table_catalog] ASC,
	[f_table_schema] ASC,
	[f_table_name] ASC,
	[f_geometry_column] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
