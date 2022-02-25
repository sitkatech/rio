CREATE TABLE [dbo].[DisadvantagedCommunityStatus](
	[DisadvantagedCommunityStatusID] [int] NOT NULL CONSTRAINT [PK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusID] PRIMARY KEY,
	[DisadvantagedCommunityStatusName] [varchar](100) NOT NULL CONSTRAINT [AK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusName] UNIQUE,
	[GeoServerLayerColor] [varchar](10) NOT NULL
)
