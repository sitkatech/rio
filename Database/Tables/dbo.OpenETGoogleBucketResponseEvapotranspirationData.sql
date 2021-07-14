SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpenETGoogleBucketResponseEvapotranspirationData](
	[OpenETGoogleBucketResponseEvapotranspirationDataID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelNumber] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[WaterMonth] [int] NOT NULL,
	[WaterYear] [int] NOT NULL,
	[EvapotranspirationRateInches] [decimal](20, 4) NULL,
 CONSTRAINT [PK_OpenETGoogleBucketResponseEvapotranspirationData_OpenETGoogleBucketResponseEvapotranspirationDataID] PRIMARY KEY CLUSTERED 
(
	[OpenETGoogleBucketResponseEvapotranspirationDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
