SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WaterTransferRegistrationParcel](
	[WaterTransferRegistrationParcelID] [int] IDENTITY(1,1) NOT NULL,
	[WaterTransferRegistrationID] [int] NOT NULL,
	[ParcelID] [int] NOT NULL,
	[AcreFeetTransferred] [int] NOT NULL,
 CONSTRAINT [PK_WaterTransferRegistrationParcel_WaterTransferRegistrationParcelID] PRIMARY KEY CLUSTERED 
(
	[WaterTransferRegistrationParcelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[WaterTransferRegistrationParcel]  WITH CHECK ADD  CONSTRAINT [FK_WaterTransferRegistrationParcel_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[WaterTransferRegistrationParcel] CHECK CONSTRAINT [FK_WaterTransferRegistrationParcel_Parcel_ParcelID]
GO
ALTER TABLE [dbo].[WaterTransferRegistrationParcel]  WITH CHECK ADD  CONSTRAINT [FK_WaterTransferRegistrationParcel_WaterTransferRegistration_WaterTransferRegistrationID] FOREIGN KEY([WaterTransferRegistrationID])
REFERENCES [dbo].[WaterTransferRegistration] ([WaterTransferRegistrationID])
GO
ALTER TABLE [dbo].[WaterTransferRegistrationParcel] CHECK CONSTRAINT [FK_WaterTransferRegistrationParcel_WaterTransferRegistration_WaterTransferRegistrationID]