CREATE TABLE [dbo].[AccountReconciliation](
	[AccountReconciliationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AccountReconciliation_AccountReconciliationID] PRIMARY KEY,
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_AccountReconciliation_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[AccountID] [int] NOT NULL CONSTRAINT [FK_AccountReconciliation_Account_AccountID] FOREIGN KEY REFERENCES [dbo].[Account] ([AccountID]),
)
