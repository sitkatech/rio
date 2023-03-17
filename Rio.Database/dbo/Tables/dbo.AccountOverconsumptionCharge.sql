CREATE TABLE [dbo].[AccountOverconsumptionCharge] (
	[AccountOverconsumptionChargeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AccountOverconsumptionCharge_AccountOverconsumptionChargeID] PRIMARY KEY,
	[AccountID] [int] NOT NULL CONSTRAINT [AK_AccountOverconsumptionCharge_Account_AccountID] FOREIGN KEY REFERENCES [dbo].[Account]([AccountID]),
	[WaterYearID] [int] NOT NULL CONSTRAINT [AK_AccountOverconsumptionCharge_WaterYear_WaterYearID] FOREIGN KEY REFERENCES [dbo].[WaterYear]([WaterYearID]),
	[OverconsumptionAmount] [decimal](10, 4) NOT NULL,
	[OverconsumptionCharge] [decimal](10, 4) NOT NULL,
	CONSTRAINT [AK_AccountOverconsumptionCharge_AccountID_WaterYearID] UNIQUE ([AccountID], [WaterYearID])
)