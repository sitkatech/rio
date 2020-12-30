alter table dbo.ParcelUpdateStaging
add ParcelGeometryText varchar(max) not null

alter table dbo.ParcelUpdateStaging
alter column ParcelGeometry geometry null