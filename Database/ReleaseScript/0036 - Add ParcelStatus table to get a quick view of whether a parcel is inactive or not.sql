create table dbo.ParcelStatus (
	ParcelStatusID int not null constraint PK_ParcelStatus_ParcelStatusID primary key,
	ParcelStatusName varchar(20) not null constraint AK_ParcelStatus_ParcelStatusName unique,
	ParcelStatusDisplayName varchar(20) not null constraint AK_ParcelStatus_ParcelStatusDisplayName unique
)

insert into dbo.ParcelStatus (ParcelStatusID, ParcelStatusName, ParcelStatusDisplayName)
values (1, 'Active', 'Active'),
(2, 'Inactive', 'Inactive')

alter table dbo.Parcel
add ParcelStatusID int null
go

update dbo.Parcel
set ParcelStatusID = 1

alter table dbo.Parcel
alter column ParcelStatusID int not null 

alter table dbo.Parcel
add constraint FK_Parcel_ParcelStatus_ParcelStatusID foreign key (ParcelStatusID) references dbo.ParcelStatus (ParcelStatusID)