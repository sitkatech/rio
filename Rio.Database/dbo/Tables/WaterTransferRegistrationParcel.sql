CREATE TABLE [dbo].[WaterTransferRegistrationParcel](
	[WaterTransferRegistrationParcelID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterTransferRegistrationParcel_WaterTransferRegistrationParcelID] PRIMARY KEY,
	[WaterTransferRegistrationID] [int] NOT NULL CONSTRAINT [FK_WaterTransferRegistrationParcel_WaterTransferRegistration_WaterTransferRegistrationID] FOREIGN KEY REFERENCES [dbo].[WaterTransferRegistration] ([WaterTransferRegistrationID]),
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_WaterTransferRegistrationParcel_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[AcreFeetTransferred] [int] NOT NULL
)