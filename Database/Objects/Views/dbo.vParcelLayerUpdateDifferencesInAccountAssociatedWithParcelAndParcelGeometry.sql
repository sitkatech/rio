if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry'))
	drop view dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
go

Create View dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
as

select coalesce(currentParcelAssociations.ParcelNumber, updatedParcelAssociations.ParcelNumber) as ParcelNumber,
		currentParcelAssociations.AccountName as OldOwnerName,
		updatedParcelAssociations.OwnerName as NewOwnerName,
		currentParcelAssociations.ParcelGeometry as OldGeometry,
		currentParcelAssociations.ParcelGeometry.STAsText() as OldGeometryText,
		updatedParcelAssociations.ParcelGeometry4326 as NewGeometry,
		updatedParcelAssociations.ParcelGeometry4326.STAsText() as NewGeometryText
from (
		select p.ParcelNumber, a.AccountName, p.ParcelGeometry
		from dbo.Parcel p
		left join dbo.vParcelOwnership vpo on p.ParcelID = vpo.ParcelID
		left join dbo.Account a on a.AccountID = vpo.AccountID
		where RowNumber = 1 or RowNumber is null
	  ) currentParcelAssociations
full outer join dbo.ParcelUpdateStaging updatedParcelAssociations on currentParcelAssociations.ParcelNumber = updatedParcelAssociations.ParcelNumber

GO