CREATE TABLE [dbo].[Well](
	[WellID] [int] NOT NULL CONSTRAINT [PK_Well_WellID] PRIMARY KEY,
	[WellGeometry] [geometry] NOT NULL,
	[WellName] [varchar](50) NOT NULL,
	[WellType] [varchar](50) NOT NULL,
	[WellTypeCode] [int] NOT NULL,
	[WellTypeCodeName] [varchar](50) NOT NULL
)
