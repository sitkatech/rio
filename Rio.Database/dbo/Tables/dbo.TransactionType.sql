CREATE TABLE [dbo].[TransactionType](
	[TransactionTypeID] [int] NOT NULL CONSTRAINT [PK_TransactionType_TransactionTypeID] PRIMARY KEY,
	[TransactionTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_TransactionType_TransactionTypeName] UNIQUE
)
