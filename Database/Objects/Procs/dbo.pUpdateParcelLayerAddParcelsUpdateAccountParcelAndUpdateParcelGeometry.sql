IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry'))
    drop procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
go

create procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
(
    @year int
)
as

begin

	insert into dbo.Parcel (ParcelNumber, ParcelGeometry, OwnerName, OwnerAddress, OwnerCity, OwnerZipCode, ParcelAreaInSquareFeet, ParcelAreaInAcres)
	select pus.ParcelNumber, pus.ParcelGeometry4326, pus.OwnerName, 'a', 'a', 'a', round(pus.ParcelGeometry.STArea() * 10.764, 0), round(pus.ParcelGeometry.STArea() / 4047, 14) 
	from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
	join dbo.ParcelUpdateStaging pus on v.ParcelNumber = pus.ParcelNumber
	where OldOwnerName is null and OldGeometryText is null

	insert into dbo.AccountParcel (AccountID, ParcelID, EffectiveYear, SaleDate)
	select a.AccountID, p.ParcelID, @year, GETDATE()
	from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
	join dbo.Parcel p on v.ParcelNumber = p.ParcelNumber
	left join dbo.Account a on v.NewOwnerName = a.AccountName
	where (OldOwnerName is null and NewOwnerName is not null) or OldOwnerName <> NewOwnerName

	update dbo.Parcel
	set ParcelGeometry = pus.ParcelGeometry4326,
	ParcelAreaInSquareFeet = round(pus.ParcelGeometry.STArea() * 10.764, 0),
	ParcelAreaInAcres = round(pus.ParcelGeometry.STArea() / 4047, 14)
	from dbo.ParcelUpdateStaging pus
	join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber

end