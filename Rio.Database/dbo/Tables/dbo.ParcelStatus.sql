CREATE TABLE [dbo].[ParcelStatus](
	[ParcelStatusID] [int] NOT NULL CONSTRAINT [PK_ParcelStatus_ParcelStatusID] PRIMARY KEY,
	[ParcelStatusName] [varchar](20) NOT NULL CONSTRAINT [AK_ParcelStatus_ParcelStatusName] UNIQUE,
	[ParcelStatusDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_ParcelStatus_ParcelStatusDisplayName] UNIQUE
)
