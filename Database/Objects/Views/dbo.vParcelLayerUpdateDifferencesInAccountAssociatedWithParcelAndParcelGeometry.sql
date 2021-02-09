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
		select p.ParcelNumber, a.AccountName, p.ParcelGeometry, apwy.WaterYearID
		from dbo.Parcel p
		left join dbo.AccountParcelWaterYear apwy on p.ParcelID = apwy.ParcelID
		left join dbo.Account a on a.AccountID = apwy.AccountID
	  ) currentParcelAssociations
full outer join dbo.ParcelUpdateStaging updatedParcelAssociations on currentParcelAssociations.ParcelNumber = updatedParcelAssociations.ParcelNumber

GO