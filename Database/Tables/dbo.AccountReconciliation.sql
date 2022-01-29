SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountReconciliation](
	[AccountReconciliationID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
 CONSTRAINT [PK_AccountReconciliation_AccountReconciliationID] PRIMARY KEY CLUSTERED 
(
	[AccountReconciliationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AccountReconciliation]  WITH CHECK ADD  CONSTRAINT [FK_AccountReconciliation_Account_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
ALTER TABLE [dbo].[AccountReconciliation] CHECK CONSTRAINT [FK_AccountReconciliation_Account_AccountID]
GO
ALTER TABLE [dbo].[AccountReconciliation]  WITH CHECK ADD  CONSTRAINT [FK_AccountReconciliation_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[AccountReconciliation] CHECK CONSTRAINT [FK_AccountReconciliation_Parcel_ParcelID]