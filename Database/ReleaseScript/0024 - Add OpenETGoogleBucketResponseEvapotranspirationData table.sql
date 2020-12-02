create table dbo.OpenETGoogleBucketResponseEvapotranspirationData (
	OpenETGoogleBucketResponseEvapotranspirationDataID int not null identity(1,1) constraint PK_OpenETGoogleBucketResponseEvapotranspirationData_OpenETGoogleBucketResponseEvapotranspirationDataID primary key,
	ParcelID int not null,
	WaterMonth int not null,
	WaterYear int not null,
	EvapotranspirationRate decimal(10,4) not null
)