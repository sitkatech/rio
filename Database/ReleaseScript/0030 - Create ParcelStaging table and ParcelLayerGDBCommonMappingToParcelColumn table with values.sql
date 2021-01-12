create table ParcelUpdateStaging (
	ParcelUpdateStagingID int not null identity(1,1) constraint PK_ParcelUpdateStaging_ParcelUpdateStagingID primary key,
	ParcelNumber varchar(20) not null constraint AK_ParcelUpdateStaging_ParcelNumber unique,
	ParcelGeometry geometry not null,
	OwnerName varchar(100) not null
)

create table ParcelLayerGDBCommonMappingToParcelStagingColumn (
	ParcelLayerGDBCommonMappingToParcelColumnID int not null  identity(1,1) constraint PK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelLayerGDBCommonMappingToParcelColumnID primary key,
	ParcelNumber varchar(100) not null constraint AK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelNumber unique,
	OwnerName varchar(100) not null constraint AK_ParcelLayerGDBCommonMappingToParcelColumn_OwnerName unique
)

insert into ParcelLayerGDBCommonMappingToParcelStagingColumn (ParcelNumber, OwnerName)
values ('APN_LABEL', 'ASSE_NAME')