create table WaterTradingScenarioWell (
	WaterTradingScenarioWellID int not null constraint PK_WaterTradingScenarioWell_WaterTradingScenarioWellID primary key,
	WaterTradingScenarioWellCountyName varchar(100) not null,
	WaterTradingScenarioWellGeometry geometry not null
)

go

insert into dbo.WaterTradingScenarioWell (WaterTradingScenarioWellID, WaterTradingScenarioWellCountyName, WaterTradingScenarioWellGeometry)
select ogr_fid, countyname, ogr_geometry
from dbo.kern_private_wells_clipped