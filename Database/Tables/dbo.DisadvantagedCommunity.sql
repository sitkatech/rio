SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DisadvantagedCommunity](
	[DisadvantagedCommunityID] [int] NOT NULL,
	[DisadvantagedCommunityName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LSADCode] [int] NOT NULL,
	[DisadvantagedCommunityStatusID] [int] NOT NULL,
	[DisadvantagedCommunityGeometry] [geometry] NOT NULL,
 CONSTRAINT [PK_DisadvantagedCommunity_DisadvantagedCommunityID] PRIMARY KEY CLUSTERED 
(
	[DisadvantagedCommunityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_DisadvantagedCommunity_DisadvantagedCommunityName_LSADCode] UNIQUE NONCLUSTERED 
(
	[DisadvantagedCommunityName] ASC,
	[LSADCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[DisadvantagedCommunity]  WITH CHECK ADD  CONSTRAINT [FK_DisadvantagedCommunity_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusID] FOREIGN KEY([DisadvantagedCommunityStatusID])
REFERENCES [dbo].[DisadvantagedCommunityStatus] ([DisadvantagedCommunityStatusID])
GO
ALTER TABLE [dbo].[DisadvantagedCommunity] CHECK CONSTRAINT [FK_DisadvantagedCommunity_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusID]