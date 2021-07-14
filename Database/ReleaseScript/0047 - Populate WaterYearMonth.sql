WITH Nbrs ( Number ) AS (
    SELECT 1 UNION ALL
    SELECT 1 + Number FROM Nbrs WHERE Number < 12
)

insert into dbo.WaterYearMonth (WaterYearID, [Month])
select WaterYearID, Number
from dbo.WaterYear wy
cross join Nbrs
order by wy.[Year], Number