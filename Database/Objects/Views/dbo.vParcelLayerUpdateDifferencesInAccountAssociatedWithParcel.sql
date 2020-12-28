if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcel'))
	drop view dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcel
go

Create View dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcel
as

select coalesce(currentParcelAssociations.ParcelNumber, updatedParcelAssociations.ParcelNumber) as ParcelNumber,
		currentParcelAssociations.AccountName as OldOwnerName,
		updatedParcelAssociations.OwnerName as NewOwnerName
from (
		select p.ParcelNumber, a.AccountName
		from dbo.Parcel p
		left join dbo.vParcelOwnership vpo on p.ParcelID = vpo.ParcelID
		left join dbo.Account a on a.AccountID = vpo.AccountID
		where RowNumber = 1 or RowNumber is null
	  ) currentParcelAssociations
full outer join dbo.ParcelUpdateStaging updatedParcelAssociations on currentParcelAssociations.ParcelNumber = updatedParcelAssociations.ParcelNumber

GO