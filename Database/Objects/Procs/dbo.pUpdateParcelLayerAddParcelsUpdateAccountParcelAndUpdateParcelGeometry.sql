IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry'))
    drop procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
go

create procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
(
    @year int
)
as

begin

	declare @squareFeetToAcresDivisor int = 43560

	--Function relies on ParcelGeometry being in EPSG:2229, or just a base geometry where the Projection's units are in feet
	insert into dbo.Parcel (ParcelNumber, ParcelGeometry, ParcelAreaInSquareFeet, ParcelAreaInAcres)
	select pus.ParcelNumber, 
		   pus.ParcelGeometry4326, 
		   round(pus.ParcelGeometry.STArea(), 0), 
		   round(pus.ParcelGeometry.STArea() / @squareFeetToAcresDivisor, 14) 
	from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
	join dbo.ParcelUpdateStaging pus on v.ParcelNumber = pus.ParcelNumber
	where OldOwnerName is null and OldGeometryText is null

	insert into dbo.AccountParcel (AccountID, ParcelID, EffectiveYear, SaleDate, ParcelStatusID)
	select a.AccountID, p.ParcelID, @year, GETDATE(), case when NewOwnerName is null then 2 else 1 end
	from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
	join dbo.Parcel p on v.ParcelNumber = p.ParcelNumber
	left join dbo.Account a on v.NewOwnerName = a.AccountName
	where (OldOwnerName is not null and NewOwnerName is null) or --deactivate case
	(OldOwnerName is null and NewOwnerName is not null) or --brand new case or existed without an owner
	OldOwnerName <> NewOwnerName
	
	update dbo.Parcel
	set ParcelGeometry = pus.ParcelGeometry4326,
	ParcelAreaInSquareFeet = round(pus.ParcelGeometry.STArea(), 0),
	ParcelAreaInAcres = round(pus.ParcelGeometry.STArea() / @squareFeetToAcresDivisor, 14)
	from dbo.ParcelUpdateStaging pus
	join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber

end