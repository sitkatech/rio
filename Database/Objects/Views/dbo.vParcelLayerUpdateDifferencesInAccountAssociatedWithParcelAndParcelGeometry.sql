if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry'))
	drop view dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
go

Create View dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
as

select coalesce(currentParcelAssociations.ParcelNumber, updatedParcelAssociations.ParcelNumber) as ParcelNumber,
		currentParcelAssociations.WaterYearID,
		currentParcelAssociations.AccountName as OldOwnerName,
		updatedParcelAssociations.OwnerName as NewOwnerName,
		currentParcelAssociations.ParcelGeometry.STAsText() as OldGeometryText,
		updatedParcelAssociations.ParcelGeometry4326.STAsText() as NewGeometryText,
		updatedParcelAssociations.HasConflict
from (
		select p.ParcelNumber, a.AccountName, p.ParcelGeometry, vpo.WaterYearID
		from dbo.vParcelOwnership vpo
		join dbo.Parcel p on p.ParcelID = vpo.ParcelID
		left join dbo.Account a on a.AccountID = vpo.AccountID
	  ) currentParcelAssociations
full outer join dbo.ParcelUpdateStaging updatedParcelAssociations on currentParcelAssociations.ParcelNumber = updatedParcelAssociations.ParcelNumber

GO