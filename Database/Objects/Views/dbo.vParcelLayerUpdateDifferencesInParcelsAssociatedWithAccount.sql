if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount'))
	drop view dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
go

Create View dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
as

select coalesce(currentAccountAssociations.AccountName, updatedAccountAssociations.OwnerName) as AccountName,
		cast(case when currentAccountAssociations.AccountName is not null then 1 else 0 end as bit) as AccountAlreadyExists,
		currentAccountAssociations.WaterYearID,
		currentAccountAssociations.ExistingParcels,
		updatedAccountAssociations.UpdatedParcels
from (
		SELECT  a.AccountName, wy.WaterYearID, STRING_AGG(po.ParcelNumber, ',') WITHIN GROUP (ORDER BY po.ParcelNumber) as ExistingParcels
		from dbo.Account a
		cross join dbo.WaterYear wy
		left join (select AccountID, ParcelNumber, apwy.WaterYearID
				   from dbo.AccountParcelWaterYear apwy
				   join dbo.Parcel p on apwy.ParcelID = p.ParcelID) po on po.AccountID = a.AccountID and po.WaterYearID = wy.WaterYearID
		group by a.AccountName, wy.WaterYearID
	  ) currentAccountAssociations
full outer join (
		select owners.OwnerName, UpdatedParcels
		from (select distinct OwnerName
			  from dbo.ParcelUpdateStaging) owners
		left join (SELECT  OwnerName, STRING_AGG(ParcelNumber, ',') WITHIN GROUP (ORDER BY ParcelNumber) as UpdatedParcels
				   from dbo.ParcelUpdateStaging
				   where HasConflict = 0
				   group by OwnerName) parcels on owners.OwnerName = parcels.OwnerName) updatedAccountAssociations on currentAccountAssociations.AccountName = updatedAccountAssociations.OwnerName

GO

