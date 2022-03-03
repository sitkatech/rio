CREATE TABLE [dbo].[OpenETGoogleBucketResponseEvapotranspirationData](
	[OpenETGoogleBucketResponseEvapotranspirationDataID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OpenETGoogleBucketResponseEvapotranspirationData_OpenETGoogleBucketResponseEvapotranspirationDataID] PRIMARY KEY,
	[ParcelNumber] [varchar](20) NOT NULL,
	[WaterMonth] [int] NOT NULL,
	[WaterYear] [int] NOT NULL,
	[EvapotranspirationRateInches] [decimal](20, 4) NULL
)
