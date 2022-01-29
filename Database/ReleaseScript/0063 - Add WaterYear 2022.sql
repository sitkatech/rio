declare @newWaterYearID int

insert into dbo.WaterYear([Year])
values(2022)

select @newWaterYearID = SCOPE_IDENTITY()

insert into dbo.WaterYearMonth(WaterYearID, [Month])
values
(@newWaterYearID, 1),
(@newWaterYearID, 2),
(@newWaterYearID, 3),
(@newWaterYearID, 4),
(@newWaterYearID, 5),
(@newWaterYearID, 6),
(@newWaterYearID, 7),
(@newWaterYearID, 8),
(@newWaterYearID, 9),
(@newWaterYearID, 10),
(@newWaterYearID, 11),
(@newWaterYearID, 12)

insert into dbo.AccountParcelWaterYear(AccountID, ParcelID, WaterYearID)
select apw.AccountID, apw.ParcelID, @newWaterYearID
from dbo.AccountParcelWaterYear apw
join dbo.WaterYear wy on apw.WaterYearID = wy.WaterYearID
where wy.[Year] = 2021