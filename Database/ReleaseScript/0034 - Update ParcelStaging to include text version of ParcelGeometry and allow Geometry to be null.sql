alter table dbo.ParcelUpdateStaging
add ParcelGeometry4326 geometry not null

alter table dbo.ParcelUpdateStaging
add ParcelGeometryText varchar(max) not null