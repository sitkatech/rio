SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WaterTransferRegistration](
	[WaterTransferRegistrationID] [int] IDENTITY(1,1) NOT NULL,
	[WaterTransferID] [int] NOT NULL,
	[WaterTransferTypeID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[WaterTransferRegistrationStatusID] [int] NOT NULL,
	[StatusDate] [datetime] NOT NULL,
 CONSTRAINT [PK_WaterTransferRegistration_WaterTransferRegistrationID] PRIMARY KEY CLUSTERED 
(
	[WaterTransferRegistrationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[WaterTransferRegistration]  WITH CHECK ADD  CONSTRAINT [FK_WaterTransferRegistration_Account_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
ALTER TABLE [dbo].[WaterTransferRegistration] CHECK CONSTRAINT [FK_WaterTransferRegistration_Account_AccountID]
GO
ALTER TABLE [dbo].[WaterTransferRegistration]  WITH CHECK ADD  CONSTRAINT [FK_WaterTransferRegistration_WaterTransfer_WaterTransferID] FOREIGN KEY([WaterTransferID])
REFERENCES [dbo].[WaterTransfer] ([WaterTransferID])
GO
ALTER TABLE [dbo].[WaterTransferRegistration] CHECK CONSTRAINT [FK_WaterTransferRegistration_WaterTransfer_WaterTransferID]
GO
ALTER TABLE [dbo].[WaterTransferRegistration]  WITH CHECK ADD  CONSTRAINT [FK_WaterTransferRegistration_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusID] FOREIGN KEY([WaterTransferRegistrationStatusID])
REFERENCES [dbo].[WaterTransferRegistrationStatus] ([WaterTransferRegistrationStatusID])
GO
ALTER TABLE [dbo].[WaterTransferRegistration] CHECK CONSTRAINT [FK_WaterTransferRegistration_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusID]
GO
ALTER TABLE [dbo].[WaterTransferRegistration]  WITH CHECK ADD  CONSTRAINT [FK_WaterTransferRegistration_WaterTransferType_WaterTransferTypeID] FOREIGN KEY([WaterTransferTypeID])
REFERENCES [dbo].[WaterTransferType] ([WaterTransferTypeID])
GO
ALTER TABLE [dbo].[WaterTransferRegistration] CHECK CONSTRAINT [FK_WaterTransferRegistration_WaterTransferType_WaterTransferTypeID]