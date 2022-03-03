CREATE TABLE [dbo].[WaterTransferRegistration](
	[WaterTransferRegistrationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterTransferRegistration_WaterTransferRegistrationID] PRIMARY KEY,
	[WaterTransferID] [int] NOT NULL CONSTRAINT [FK_WaterTransferRegistration_WaterTransfer_WaterTransferID] FOREIGN KEY REFERENCES [dbo].[WaterTransfer] ([WaterTransferID]),
	[WaterTransferTypeID] [int] NOT NULL CONSTRAINT [FK_WaterTransferRegistration_WaterTransferType_WaterTransferTypeID] FOREIGN KEY REFERENCES [dbo].[WaterTransferType] ([WaterTransferTypeID]),
	[AccountID] [int] NOT NULL CONSTRAINT [FK_WaterTransferRegistration_Account_AccountID] FOREIGN KEY REFERENCES [dbo].[Account] ([AccountID]),
	[WaterTransferRegistrationStatusID] [int] NOT NULL CONSTRAINT [FK_WaterTransferRegistration_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusID] FOREIGN KEY REFERENCES [dbo].[WaterTransferRegistrationStatus] ([WaterTransferRegistrationStatusID]),
	[StatusDate] [datetime] NOT NULL
)