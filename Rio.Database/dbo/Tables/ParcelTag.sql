CREATE TABLE [dbo].[ParcelTag](
	[ParcelTagID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelTag_ParcelTagID] PRIMARY KEY,
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_ParcelTag_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[TagID] [int] NOT NULL CONSTRAINT [FK_ParcelTag_Tag_TagID] FOREIGN KEY REFERENCES [dbo].[Tag] ([TagID])
)