create table #new_years
(
	[Year] int not null
)

insert into #new_years ([Year]) 
(
	select (y.WaterYear) from (
	select 2018 as WaterYear union 
	select 2019 union
	select 2020 union
	select 2021 union
	select 2022
	) y left join dbo.WaterYear wy on y.WaterYear = wy.[Year] 
	where wy.WaterYearID is null
)
go

insert into dbo.WaterYear([Year]) 
(
	select #new_years.[Year] from #new_years
)
go

WITH Nbrs ( Number ) AS (
    SELECT 1 UNION ALL
    SELECT 1 + Number FROM Nbrs WHERE Number < 12
)

insert into dbo.WaterYearMonth (WaterYearID, [Month])
select wy.WaterYearID, Number
from (
	select * from dbo.WaterYear
	where WaterYear.[Year] in 
	( select #new_years.[Year] from #new_years)
) as wy
cross join Nbrs
order by wy.[Year], Number

drop table #new_years