SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountParcel](
	[AccountParcelID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NULL,
	[ParcelID] [int] NOT NULL,
	[OwnerName] [varchar](214) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EffectiveYear] [int] NULL,
	[SaleDate] [datetime] NULL,
	[Note] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ParcelStatusID] [int] NOT NULL,
 CONSTRAINT [PK_AccountParcel_AccountParcelID] PRIMARY KEY CLUSTERED 
(
	[AccountParcelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AccountParcel]  WITH CHECK ADD  CONSTRAINT [FK_AccountParcel_Account_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
ALTER TABLE [dbo].[AccountParcel] CHECK CONSTRAINT [FK_AccountParcel_Account_AccountID]
GO
ALTER TABLE [dbo].[AccountParcel]  WITH CHECK ADD  CONSTRAINT [FK_AccountParcel_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[AccountParcel] CHECK CONSTRAINT [FK_AccountParcel_Parcel_ParcelID]
GO
ALTER TABLE [dbo].[AccountParcel]  WITH CHECK ADD  CONSTRAINT [FK_AccountParcel_ParcelStatus_ParcelStatusID] FOREIGN KEY([ParcelStatusID])
REFERENCES [dbo].[ParcelStatus] ([ParcelStatusID])
GO
ALTER TABLE [dbo].[AccountParcel] CHECK CONSTRAINT [FK_AccountParcel_ParcelStatus_ParcelStatusID]
GO
ALTER TABLE [dbo].[AccountParcel]  WITH CHECK ADD  CONSTRAINT [CK_ParcelOwner_OwnerNameXorOwnerIDXorParcelStatusID] CHECK  (([ParcelStatusID]=(2) OR [AccountID] IS NULL AND [OwnerName] IS NOT NULL OR [AccountID] IS NOT NULL AND [OwnerName] IS NULL))
GO
ALTER TABLE [dbo].[AccountParcel] CHECK CONSTRAINT [CK_ParcelOwner_OwnerNameXorOwnerIDXorParcelStatusID]