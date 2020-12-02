SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpenETGoogleBucketResponseEvapotranspirationData](
	[OpenETGoogleBucketResponseEvapotranspirationDataID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelID] [int] NOT NULL,
	[WaterMonth] [int] NOT NULL,
	[WaterYear] [int] NOT NULL,
	[EvapotranspirationRate] [decimal](10, 4) NOT NULL,
 CONSTRAINT [PK_OpenETGoogleBucketResponseEvapotranspirationData_OpenETGoogleBucketResponseEvapotranspirationDataID] PRIMARY KEY CLUSTERED 
(
	[OpenETGoogleBucketResponseEvapotranspirationDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
