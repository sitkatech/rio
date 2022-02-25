CREATE TABLE [dbo].[DisadvantagedCommunity](
	[DisadvantagedCommunityID] [int] NOT NULL CONSTRAINT [PK_DisadvantagedCommunity_DisadvantagedCommunityID] PRIMARY KEY,
	[DisadvantagedCommunityName] [varchar](100) NOT NULL,
	[LSADCode] [int] NOT NULL,
	[DisadvantagedCommunityStatusID] [int] NOT NULL CONSTRAINT [FK_DisadvantagedCommunity_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusID] FOREIGN KEY REFERENCES [dbo].[DisadvantagedCommunityStatus] ([DisadvantagedCommunityStatusID]),
	[DisadvantagedCommunityGeometry] [geometry] NOT NULL,
	CONSTRAINT [AK_DisadvantagedCommunity_DisadvantagedCommunityName_LSADCode] UNIQUE ([DisadvantagedCommunityName], [LSADCode])
)