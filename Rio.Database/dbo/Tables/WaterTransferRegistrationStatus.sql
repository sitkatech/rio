CREATE TABLE [dbo].[WaterTransferRegistrationStatus](
	[WaterTransferRegistrationStatusID] [int] NOT NULL CONSTRAINT [PK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusID] PRIMARY KEY,
	[WaterTransferRegistrationStatusName] [varchar](100) NOT NULL CONSTRAINT [AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusName] UNIQUE,
	[WaterTransferRegistrationStatusDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusDisplayName] UNIQUE,
)
