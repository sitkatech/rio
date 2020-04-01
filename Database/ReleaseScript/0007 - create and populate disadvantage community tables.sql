create table
dbo.DisadvantagedCommunityStatus (
	DisadvantagedCommunityStatusID int not null constraint PK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusID primary key,
	DisadvantagedCommunityStatusName varchar(100) not null constraint AK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusName unique,
	GeoServerLayerColor varchar(10) not null
)

create table
dbo.DisadvantagedCommunity(
	DisadvantagedCommunityID int not null constraint PK_DisadvantagedCommunity_DisadvantagedCommunityID primary key,
	DisadvantagedCommunityName varchar(100) not null,
	LSADCode int not null,
	DisadvantagedCommunityStatusID int not null constraint FK_DisadvantagedCommunity_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusID foreign key references dbo.DisadvantagedCommunityStatus(DisadvantagedCommunityStatusID),
	DisadvantagedCommunityGeometry geometry not null,
	constraint AK_DisadvantagedCommunity_DisadvantagedCommunityName_LSADCode unique (DisadvantagedCommunityName, LSADCode)
)

go

insert into dbo.DisadvantagedCommunityStatus (DisadvantagedCommunityStatusID, DisadvantagedCommunityStatusName, GeoServerLayerColor)
values (1, 'Disadvantaged Community', '#000000'),
(2, 'Severely Disadvantaged Community', '#000000')

insert into dbo.DisadvantagedCommunity(DisadvantagedCommunityID, DisadvantagedCommunityName, LSADCode, DisadvantagedCommunityStatusID, DisadvantagedCommunityGeometry)
select dl.objectid, dl.[name], dl.lsad, case when dl.dac_status = 'Disadvantaged Community' then 1 when dl.dac_status = 'Severely Disadvantaged Community' then 2 else null end, ogr_geometry
from dbo.dac_layer dl