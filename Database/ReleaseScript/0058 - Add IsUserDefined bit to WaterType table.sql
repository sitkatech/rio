alter table dbo.WaterType
add IsUserDefined bit not null default 1

go 

-- Precip not user defined
update dbo.WaterType
set IsUserDefined = 0
where WaterTypeID = 5