﻿IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pParcelAllocationAndUsage'))
    drop procedure dbo.pParcelAllocationAndUsage
go

create procedure dbo.pParcelAllocationAndUsage
(
    @year int
)
as

begin

	select p.ParcelID, p.ParcelNumber, p.ParcelAreaInAcres, u.UserID, u.FirstName, u.LastName, u.Email,
			pal.Allocation, pal.ProjectWater, pal.Reconciliation, pal.NativeYield, pal.StoredWater,
			pmev.UsageToDate, up.OwnerName
	from dbo.Parcel p
	left join (
        select
            *, 
            Row_Number() OVER (
		        Partition by ParcelID Order by isnull(SaleDate,0) desc
	        ) as RowNumber
        from
            dbo.UserParcel
        where
            (SaleDate is null or Year(SaleDate) <= @year) and
            (EffectiveYear is null or EffectiveYear <= @year)
    ) up on p.ParcelID = up.ParcelID
	left join dbo.[User] u on up.UserID = u.UserID
	left join 
	(
		select pa.ParcelID, 
			isnull(sum(pa.AcreFeetAllocated), 0) as Allocation, 
			sum(case when pa.ParcelAllocationTypeID = 1 then pa.AcreFeetAllocated else 0 end) as ProjectWater,
			sum(case when pa.ParcelAllocationTypeID = 2 then pa.AcreFeetAllocated else 0 end) as Reconciliation,
			sum(case when pa.ParcelAllocationTypeID = 3 then pa.AcreFeetAllocated else 0 end) as NativeYield,
			sum(case when pa.ParcelAllocationTypeID = 4 then pa.AcreFeetAllocated else 0 end) as StoredWater
		from dbo.ParcelAllocation pa where pa.WaterYear = @year
		group by pa.ParcelID
	) pal on p.ParcelID = pal.ParcelID
	left join 
	(
		select pme.ParcelID, isnull(sum(pme.EvapotranspirationRate), 0) as UsageToDate
		from dbo.ParcelMonthlyEvapotranspiration pme where pme.WaterYear = @year
		group by pme.ParcelID
	) pmev on p.ParcelID = pmev.ParcelID

    where RowNumber = 1

end