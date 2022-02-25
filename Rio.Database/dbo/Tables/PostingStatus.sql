CREATE TABLE [dbo].[PostingStatus](
	[PostingStatusID] [int] NOT NULL CONSTRAINT [PK_PostingStatus_PostingStatusID] PRIMARY KEY,
	[PostingStatusName] [varchar](100) NOT NULL CONSTRAINT [AK_PostingStatus_PostingStatusName] UNIQUE,
	[PostingStatusDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_PostingStatus_PostingStatusDisplayName] UNIQUE
)
