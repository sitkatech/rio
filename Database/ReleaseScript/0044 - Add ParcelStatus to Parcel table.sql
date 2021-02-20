alter table dbo.Parcel
add ParcelStatusID int null
go

alter table dbo.Parcel
add InactivateDate datetime null
go

update dbo.Parcel
set ParcelStatusID = 1

alter table dbo.Parcel
alter column ParcelStatusID int not null 

alter table dbo.Parcel
add constraint FK_Parcel_ParcelStatus_ParcelStatusID foreign key ([ParcelStatusID]) references dbo.ParcelStatus (ParcelStatusID)

alter table dbo.Parcel
with check add constraint CK_ParcelStatus_ActiveXorInactiveAndInactivateDate CHECK ([ParcelStatusID] = 1 or ([ParcelStatusID] = 2 and InactivateDate is not null))