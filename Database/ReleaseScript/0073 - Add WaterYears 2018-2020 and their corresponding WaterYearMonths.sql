insert into dbo.WaterYear([Year])
values (2018), (2019), (2020)
go

WITH Nbrs ( Number ) AS (
    SELECT 1 UNION ALL
    SELECT 1 + Number FROM Nbrs WHERE Number < 12
)

insert into dbo.WaterYearMonth (WaterYearID, [Month])
select wy.WaterYearID, Number
from (
select * from dbo.WaterYear
where [Year] < 2021
) as wy
cross join Nbrs
order by wy.[Year], Number