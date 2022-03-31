CREATE TABLE [dbo].[WaterYear](
	[WaterYearID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterYear_WaterYearID] PRIMARY KEY,
	[Year] [int] NOT NULL CONSTRAINT [AK_WaterYear_Year] UNIQUE,
	[ParcelLayerUpdateDate] [datetime] NULL
)
