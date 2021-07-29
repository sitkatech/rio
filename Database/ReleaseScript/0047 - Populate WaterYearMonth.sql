WITH Nbrs ( Number ) AS (
    SELECT 1 UNION ALL
    SELECT 1 + Number FROM Nbrs WHERE Number < 12
)

insert into dbo.WaterYearMonth (WaterYearID, [Month], FinalizeDate)
select WaterYearID, Number, case when wy.[Year] < 2020 then '2021-07-16' else null end
from dbo.WaterYear wy
cross join Nbrs
order by wy.[Year], Number