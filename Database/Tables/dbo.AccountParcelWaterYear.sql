SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountParcelWaterYear](
	[AccountParcelWaterYearID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[ParcelID] [int] NOT NULL,
	[WaterYearID] [int] NOT NULL,
 CONSTRAINT [PK_AccountParcelWaterYear_AccountParcelWaterYear] PRIMARY KEY CLUSTERED 
(
	[AccountParcelWaterYearID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_AccountParcelWaterYear_ParcelID_WaterYearID] UNIQUE NONCLUSTERED 
(
	[ParcelID] ASC,
	[WaterYearID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AccountParcelWaterYear]  WITH CHECK ADD  CONSTRAINT [FK_AccountParcelWaterYear_Account_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
ALTER TABLE [dbo].[AccountParcelWaterYear] CHECK CONSTRAINT [FK_AccountParcelWaterYear_Account_AccountID]
GO
ALTER TABLE [dbo].[AccountParcelWaterYear]  WITH CHECK ADD  CONSTRAINT [FK_AccountParcelWaterYear_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[AccountParcelWaterYear] CHECK CONSTRAINT [FK_AccountParcelWaterYear_Parcel_ParcelID]
GO
ALTER TABLE [dbo].[AccountParcelWaterYear]  WITH CHECK ADD  CONSTRAINT [FK_AccountParcelWaterYear_WaterYear_WaterYearID] FOREIGN KEY([WaterYearID])
REFERENCES [dbo].[WaterYear] ([WaterYearID])
GO
ALTER TABLE [dbo].[AccountParcelWaterYear] CHECK CONSTRAINT [FK_AccountParcelWaterYear_WaterYear_WaterYearID]