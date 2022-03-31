CREATE TABLE [dbo].[spatial_ref_sys](
	[srid] [int] NOT NULL constraint PK_spatial_ref_sys_srid PRIMARY KEY,
	[auth_name] [varchar](256) NULL,
	[auth_srid] [int] NULL,
	[srtext] [varchar](2048) NULL,
	[proj4text] [varchar](2048) NULL,
)
