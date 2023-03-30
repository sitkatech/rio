CREATE TABLE [dbo].[ParcelUsageStaging](
	[ParcelUsageStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelUsageStaging_ParcelUsageStagingID] PRIMARY KEY,
	[ParcelID] [int] NULL CONSTRAINT [FK_ParcelUsageStaging_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel]([ParcelID]),
	[ParcelNumber] [varchar](20) NOT NULL,
	[ReportedDate] [datetime] NOT NULL,
	[ReportedValue] [decimal](20, 4) NOT NULL,
	[ReportedValueInAcreFeet] [decimal](20, 4) NOT NULL,
	[ParcelUsageFileUploadID] [int] NOT NULL CONSTRAINT [FK_ParcelUsagStaging_ParcelUsageFileUpload_ParcelUsageFileUploadID] FOREIGN KEY REFERENCES [dbo].[ParcelUsageFileUpload]([ParcelUsageFileUploadID]),
	[UserID] [int] NOT NULL CONSTRAINT [FK_ParcelUsageStaging_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),

)
