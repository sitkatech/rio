if exists (select * from dbo.sysobjects where id = object_id('dbo.vAllParcelsWithAnnualWaterUsage'))
	drop view dbo.vAllParcelsWithAnnualWaterUsage
go

create view dbo.vAllParcelsWithAnnualWaterUsage
as

select ParcelID, ParcelNumber, ParcelAreaInAcres, UserID, FirstName, LastName, Email, 
[2016] as WaterUsageFor2016, [2017]  as WaterUsageFor2017, [2018] as WaterUsageFor2018
from
(
	select p.ParcelID, p.ParcelNumber, p.ParcelAreaInAcres, u.UserID, u.FirstName, u.LastName, u.Email, b.WaterYear, b.WaterUsage
	from dbo.Parcel p
	left join dbo.UserParcel up on p.ParcelID = up.ParcelID
	left join dbo.[User] u on up.UserID = u.UserID
	left join
	(
		select a.ParcelID, a.WaterYear, sum(a.WaterUsage) as WaterUsage
		from
		(
			select pa.ParcelID, pa.WaterYear, sum(pa.AcreFeetAllocated) as WaterUsage
			from dbo.ParcelAllocation pa
			group by pa.ParcelID, pa.WaterYear
			union all
			select pa.ParcelID, pa.WaterYear, -sum(pa.EvapotranspirationRate) as WaterUsage
			from dbo.ParcelMonthlyEvapotranspiration pa
			group by pa.ParcelID, pa.WaterYear
		) a
		group by a.ParcelID, a.WaterYear
	) b on p.ParcelID = b.ParcelID
) c
pivot
(
sum(c.WaterUsage)
for WaterYear in ([2016],[2017],[2018])
) as PivotTable

GO
/*
select *
from dbo.vAllParcelsWithAnnualWaterUsage
*/