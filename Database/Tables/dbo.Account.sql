SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[AccountNumber]  AS (isnull([AccountID]+(10000),(0))),
	[AccountName] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AccountStatusID] [int] NOT NULL,
	[Notes] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UpdateDate] [datetime] NULL,
	[AccountVerificationKey] [varchar](6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AccountVerificationKeyLastUseDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL,
	[InactivateDate] [datetime] NULL,
 CONSTRAINT [PK_Account_AccountID] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_Account_AccountNumber] UNIQUE NONCLUSTERED 
(
	[AccountNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Account_AccountVerificationKey] ON [dbo].[Account]
(
	[AccountVerificationKey] ASC
)
WHERE ([AccountVerificationKey] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_AccountStatus_AccountStatusID] FOREIGN KEY([AccountStatusID])
REFERENCES [dbo].[AccountStatus] ([AccountStatusID])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_AccountStatus_AccountStatusID]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [CK_InactivateDate_AccountStatusInactive] CHECK  (([AccountStatusID]=(2) AND [InactivateDate] IS NOT NULL OR [AccountStatusID]<>(2) AND [InactivateDate] IS NULL))
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [CK_InactivateDate_AccountStatusInactive]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [CK_InactiveDate_ParcelStatusIDXorInactivateDate] CHECK  (([AccountStatusID]=(2) AND [InactivateDate] IS NOT NULL OR [AccountStatusID]<>(2) AND [InactivateDate] IS NULL))
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [CK_InactiveDate_ParcelStatusIDXorInactivateDate]