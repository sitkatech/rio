SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CimisPrecipitationDatum](
	[CimisPrecipitationDatumID] [int] IDENTITY(1,1) NOT NULL,
	[DateMeasured] [datetime] NOT NULL,
	[Precipitation] [decimal](5, 2) NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_CimisPrecipitationDatum_CimisPrecipitationDatumID] PRIMARY KEY CLUSTERED 
(
	[CimisPrecipitationDatumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
