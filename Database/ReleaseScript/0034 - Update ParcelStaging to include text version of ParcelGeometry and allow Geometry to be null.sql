alter table dbo.ParcelUpdateStaging
add ParcelGeometry4326 geometry null

alter table dbo.ParcelUpdateStaging
alter column ParcelGeometry geometry null

alter table dbo.ParcelUpdateStaging
add ParcelGeometryText varchar(max) not null

alter table dbo.ParcelUpdateStaging
add ParcelGeometry4326Text varchar(max) not null