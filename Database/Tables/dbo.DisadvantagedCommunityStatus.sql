SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DisadvantagedCommunityStatus](
	[DisadvantagedCommunityStatusID] [int] NOT NULL,
	[DisadvantagedCommunityStatusName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[GeoServerLayerColor] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusID] PRIMARY KEY CLUSTERED 
(
	[DisadvantagedCommunityStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusName] UNIQUE NONCLUSTERED 
(
	[DisadvantagedCommunityStatusName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
