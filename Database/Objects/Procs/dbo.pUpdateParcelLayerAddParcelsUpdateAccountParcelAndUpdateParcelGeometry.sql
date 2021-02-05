IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry'))
    drop procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
go

create procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
(
    @waterYearID int
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
	from (select ParcelNumber, OldOwnerName, OldGeometryText
		  from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
		  where WaterYearID = @waterYearID or WaterYearID is null) v
	join dbo.ParcelUpdateStaging pus on v.ParcelNumber = pus.ParcelNumber
	where OldOwnerName is null and OldGeometryText is null

	delete from dbo.AccountParcelWaterYear
	where WaterYearID in (select WaterYearID
						  from dbo.WaterYear
						  where [Year] >= (select [Year]
										  from dbo.WaterYear
										  where WaterYearID = @waterYearID))

	insert into dbo.AccountParcelWaterYear (AccountID, ParcelID, WaterYearID)
	select a.AccountID, p.ParcelID, wy.WaterYearID
	from (select ParcelNumber, NewOwnerName
		  from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
		  where WaterYearID = @waterYearID or WaterYearID is null) v
	join dbo.Parcel p on v.ParcelNumber = p.ParcelNumber
	join dbo.Account a on v.NewOwnerName = a.AccountName
	cross join (select WaterYearID
						  from dbo.WaterYear
						  where [Year] >= (select [Year]
										  from dbo.WaterYear
										  where WaterYearID = @waterYearID)) wy
	
	update dbo.Parcel
	set ParcelGeometry = pus.ParcelGeometry4326,
	ParcelAreaInSquareFeet = round(pus.ParcelGeometry.STArea(), 0),
	ParcelAreaInAcres = round(pus.ParcelGeometry.STArea() / @squareFeetToAcresDivisor, 14)
	from dbo.ParcelUpdateStaging pus
	join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber

end