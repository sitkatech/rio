if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount'))
	drop view dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
go

Create View dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
as

select coalesce(currentAccountAssociations.AccountName, updatedAccountAssociations.OwnerName) as AccountName,
		cast(case when currentAccountAssociations.AccountName is not null then 1 else 0 end as bit) as AccountAlreadyExists,
		currentAccountAssociations.ExistingParcels,
		updatedAccountAssociations.UpdatedParcels
from (
		SELECT  a.AccountName, STRING_AGG(po.ParcelNumber, ',') WITHIN GROUP (ORDER BY po.ParcelNumber) as ExistingParcels
		from dbo.Account a
		left join (select AccountID, ParcelNumber
				   from dbo.vParcelOwnership po
				   join dbo.Parcel p on po.ParcelID = p.ParcelID
				   where RowNumber = 1) po on po.AccountID = a.AccountID
		group by a.AccountName
	  ) currentAccountAssociations
full outer join (
		SELECT  OwnerName, STRING_AGG(ParcelNumber, ',') WITHIN GROUP (ORDER BY ParcelNumber) as UpdatedParcels
		from dbo.ParcelUpdateStaging
		group by OwnerName) updatedAccountAssociations on currentAccountAssociations.AccountName = updatedAccountAssociations.OwnerName

GO

