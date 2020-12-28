if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount'))
	drop view dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
go

Create View dbo.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
as

select coalesce(currentAccountAssociations.AccountName, updatedAccountAssociations.OwnerName) as AccountName,
		Left(currentAccountAssociations.Parcels, len(currentAccountAssociations.Parcels) - 1) as ExistingParcels,
		Left(updatedAccountAssociations.Parcels, len(updatedAccountAssociations.Parcels) -1) as UpdatedParcels
from (
		SELECT  a.AccountName,
            (
                SELECT  p.ParcelNumber + ', ' AS [text()]
                FROM    dbo.vParcelOwnership ap
				join dbo.Parcel p on ap.ParcelID = p.ParcelID
                WHERE   ap.AccountID = a.AccountID and ap.RowNumber = 1
                order by p.ParcelNumber
                FOR XML PATH(''), TYPE
                ).value('/', 'NVARCHAR(MAX)'
			) as Parcels
		from dbo.Account a
	  ) currentAccountAssociations
full outer join (
		SELECT  a.OwnerName,
            (
                SELECT  pus.ParcelNumber + ', ' AS [text()]
                FROM    dbo.ParcelUpdateStaging pus
                WHERE   pus.OwnerName = a.OwnerName
                order by pus.ParcelNumber
                FOR XML PATH(''), TYPE
                ).value('/', 'NVARCHAR(MAX)'
			) as Parcels
		from dbo.ParcelUpdateStaging a
		group by a.OwnerName) updatedAccountAssociations on currentAccountAssociations.AccountName = updatedAccountAssociations.OwnerName

GO

