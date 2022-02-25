CREATE TABLE [dbo].[spatial_ref_sys](
	[srid] [int] NOT NULL PRIMARY KEY CLUSTERED,
	[auth_name] [varchar](256) NULL,
	[auth_srid] [int] NULL,
	[srtext] [varchar](2048) NULL,
	[proj4text] [varchar](2048) NULL,
)
