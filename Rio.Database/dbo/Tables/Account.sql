CREATE TABLE [dbo].[Account](
	[AccountID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Account_AccountID] PRIMARY KEY,
	[AccountNumber]  AS (isnull([AccountID]+(10000),(0))) CONSTRAINT [AK_Account_AccountNumber] UNIQUE,
	[AccountName] [varchar](255) NULL,
	[AccountStatusID] [int] NOT NULL CONSTRAINT [FK_Account_AccountStatus_AccountStatusID] FOREIGN KEY REFERENCES [dbo].[AccountStatus] ([AccountStatusID]),
	[Notes] [varchar](max) NULL,
	[UpdateDate] [datetime] NULL,
	[AccountVerificationKey] [varchar](6) NULL,
	[AccountVerificationKeyLastUseDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL,
	[InactivateDate] [datetime] NULL,
	CONSTRAINT [CK_InactivateDate_AccountStatusInactive] CHECK  (([AccountStatusID]=(2) AND [InactivateDate] IS NOT NULL OR [AccountStatusID]<>(2) AND [InactivateDate] IS NULL)),
	CONSTRAINT [CK_InactiveDate_ParcelStatusIDXorInactivateDate] CHECK  (([AccountStatusID]=(2) AND [InactivateDate] IS NOT NULL OR [AccountStatusID]<>(2) AND [InactivateDate] IS NULL)))
GO

CREATE UNIQUE NONCLUSTERED INDEX [AK_Account_AccountVerificationKey] ON [dbo].[Account]
(
	[AccountVerificationKey] ASC
)
WHERE ([AccountVerificationKey] IS NOT NULL)
GO
