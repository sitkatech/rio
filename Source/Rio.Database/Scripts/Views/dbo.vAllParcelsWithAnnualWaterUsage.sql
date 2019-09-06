if exists (select * from dbo.sysobjects where id = object_id('dbo.vAllParcelsWithAnnualWaterUsage'))
	drop view dbo.vAllParcelsWithAnnualWaterUsage
go

create view dbo.vAllParcelsWithAnnualWaterUsage
as

select p.ParcelID, p.ParcelNumber, p.ParcelAreaInAcres, u.UserID, u.FirstName, u.LastName, u.Email
, palloc.AllocationFor2016, palloc.AllocationFor2017, palloc.AllocationFor2018
, pwu.WaterUsageFor2016, pwu.WaterUsageFor2017, pwu.WaterUsageFor2018
from dbo.Parcel p
left join dbo.UserParcel up on p.ParcelID = up.ParcelID
left join dbo.[User] u on up.UserID = u.UserID
left join
(
	select ParcelID, [2016] as AllocationFor2016, [2017]  as AllocationFor2017, [2018] as AllocationFor2018
	from
	(
		select pa.ParcelID, pa.WaterYear, sum(pa.AcreFeetAllocated) as AcreFeetAllocated
		from dbo.ParcelAllocation pa
		group by pa.ParcelID, pa.WaterYear
	) c
	pivot
	(
		sum(c.AcreFeetAllocated)
		for WaterYear in ([2016],[2017],[2018])
	) as PivotTable
) palloc on p.ParcelID = palloc.ParcelID
left join
(
	select ParcelID, [2016] as WaterUsageFor2016, [2017]  as WaterUsageFor2017, [2018] as WaterUsageFor2018
	from
	(
		select pa.ParcelID, pa.WaterYear, -sum(pa.EvapotranspirationRate) as WaterUsage
		from dbo.ParcelMonthlyEvapotranspiration pa
		group by pa.ParcelID, pa.WaterYear
	) c
	pivot
	(
		sum(c.WaterUsage)
		for WaterYear in ([2016],[2017],[2018])
	) as PivotTable
) pwu on p.ParcelID = pwu.ParcelID

GO
/*
select *
from dbo.vAllParcelsWithAnnualWaterUsage
*/