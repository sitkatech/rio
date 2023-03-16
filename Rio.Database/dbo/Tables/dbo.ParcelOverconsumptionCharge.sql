CREATE TABLE [dbo].[ParcelOverconsumptionCharge] (
	[ParcelOverconsumptionChargeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelOverconsumptionCharge_ParcelOverconsumptionChargeID] PRIMARY KEY,
	[ParcelID] [int] NOT NULL CONSTRAINT [AK_ParcelOverconsumptionCharge_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel]([ParcelID]),
	[WaterYearID] [int] NOT NULL CONSTRAINT [AK_ParcelOverconsumptionCharge_WaterYear_WaterYearID] FOREIGN KEY REFERENCES [dbo].[WaterYear]([WaterYearID]),
	[OverconsumptionAmount] [decimal](10, 4) NOT NULL,
	[OverconsumptionCharge] [decimal](10, 4) NOT NULL,
	CONSTRAINT [AK_ParcelOverconsumptionCharge_ParcelID_WaterYearID] UNIQUE ([ParcelID], [WaterYearID])
)