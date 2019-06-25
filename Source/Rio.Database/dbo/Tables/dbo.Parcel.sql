create table dbo.Parcel
(
	ParcelID int not null identity(1,1) constraint PK_Parcel_ParcelID primary key,
	ParcelNumber varchar(20) not null constraint AK_Parcel_ParcelNumber unique,
	ParcelGeometry geometry not null,
	OwnerName varchar(100) not null,
	OwnerAddress varchar(100) not null,
	OwnerCity varchar(100) not null,
	OwnerZipCode varchar(20) not null,
	ParcelAreaInSquareFeet int not null,
	ParcelAreaInAcres float not null
)
GO
