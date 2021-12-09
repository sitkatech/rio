alter table dbo.WaterType
add IsUserDefined bit null

go 

update dbo.WaterType set IsUserDefined = 1
alter table dbo.WaterType alter column IsUserDefined bit not null

-- Precip not user defined
update dbo.WaterType
set IsUserDefined = 0
where WaterTypeID = 5
