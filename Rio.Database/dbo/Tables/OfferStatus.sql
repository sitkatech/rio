CREATE TABLE [dbo].[OfferStatus](
	[OfferStatusID] [int] NOT NULL CONSTRAINT [PK_OfferStatus_OfferStatusID] PRIMARY KEY,
	[OfferStatusName] [varchar](100) NOT NULL CONSTRAINT [AK_OfferStatus_OfferStatusName] UNIQUE,
	[OfferStatusDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_OfferStatus_OfferStatusDisplayName] UNIQUE
)
