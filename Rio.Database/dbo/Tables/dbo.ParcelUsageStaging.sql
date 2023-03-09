CREATE TABLE [dbo].[ParcelUsageStaging](
	[ParcelUsageStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelUsage_ParcelUsageID] PRIMARY KEY,
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_ParcelUsageStaging_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel]([ParcelID]),
	[ParcelNumber] [varchar](20) NOT NULL,
	[ReportedDate] [datetime] NOT NULL,
	[ReportedValue] [decimal](20, 4) NOT NULL,
	[ReportedValueInAcreFeet] [decimal](20, 4) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
	[UploadedFileName] [varchar](100) NOT NULL,
	[UserID] [int] NOT NULL CONSTRAINT [FK_ParcelUsageStaging_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
)
