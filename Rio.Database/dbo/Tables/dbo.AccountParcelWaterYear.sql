CREATE TABLE [dbo].[AccountParcelWaterYear](
	[AccountParcelWaterYearID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AccountParcelWaterYear_AccountParcelWaterYear] PRIMARY KEY,
	[AccountID] [int] NOT NULL CONSTRAINT [FK_AccountParcelWaterYear_Account_AccountID] FOREIGN KEY REFERENCES [dbo].[Account] ([AccountID]),
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_AccountParcelWaterYear_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[WaterYearID] [int] NOT NULL CONSTRAINT [FK_AccountParcelWaterYear_WaterYear_WaterYearID] FOREIGN KEY REFERENCES [dbo].[WaterYear] ([WaterYearID]),
	CONSTRAINT [AK_AccountParcelWaterYear_ParcelID_WaterYearID] UNIQUE ([ParcelID], [WaterYearID])
)