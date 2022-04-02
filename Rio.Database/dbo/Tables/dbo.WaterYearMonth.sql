CREATE TABLE [dbo].[WaterYearMonth](
	[WaterYearMonthID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterYearMonth_WaterYearMonthID] PRIMARY KEY,
	[WaterYearID] [int] NOT NULL CONSTRAINT [FK_WaterYearMonth_WaterYear_WaterYearID] FOREIGN KEY REFERENCES [dbo].[WaterYear] ([WaterYearID]),
	[Month] [int] NOT NULL,
	[FinalizeDate] [datetime] NULL,
	CONSTRAINT [AK_WaterYearMonth_WaterYearID_Month] UNIQUE ([WaterYearID], [Month])
)