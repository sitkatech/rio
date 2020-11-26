create table dbo.OpenETGoogleBucketResponseEvapotranspirationData (
	OpenETGoogleBucketResponseEvapotranspirationDataID int not null identity(1,1) constraint PK_OpenETGoogleBucketResponseEvapotranspirationData_OpenETGoogleBucketResponseEvapotranspirationDataID primary key,
	ParcelNumber varchar(20),
	WaterMonth int not null,
	WaterYear int not null,
	EvapotranspirationRate decimal(10,4) not null
)