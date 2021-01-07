delete 
from dbo.ParcelAllocation
where ParcelID in (
select p.ParcelID
from dbo.Parcel p
left join dbo.AccountParcel ap on ap.ParcelID = p.ParcelID
where ap.ParcelID is null)

delete 
from dbo.ParcelMonthlyEvapotranspiration
where ParcelID in (
select p.ParcelID
from dbo.Parcel p
left join dbo.AccountParcel ap on ap.ParcelID = p.ParcelID
where ap.ParcelID is null)

delete 
from dbo.Parcel
where ParcelID in (
select p.ParcelID
from dbo.Parcel p
left join dbo.AccountParcel ap on ap.ParcelID = p.ParcelID
where ap.ParcelID is null)