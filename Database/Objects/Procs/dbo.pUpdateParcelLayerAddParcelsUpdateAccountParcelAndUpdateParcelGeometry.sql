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
	from dbo.ParcelUpdateStaging pus
	join (select max(ParcelUpdateStagingID) ParcelUpdateStagingID
		  from dbo.ParcelUpdateStaging
		  group by ParcelNumber) pusu on pusu.ParcelUpdateStagingID = pus.ParcelUpdateStagingID
	left join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber
	where p.ParcelID is null

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
		  where (WaterYearID = @waterYearID or WaterYearID is null) and HasConflict = 0) v
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
	join (select max(ParcelUpdateStagingID) ParcelUpdateStagingID
		  from dbo.ParcelUpdateStaging
		  group by ParcelNumber) pusu on pusu.ParcelUpdateStagingID = pus.ParcelUpdateStagingID
	join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber

	insert into dbo.AccountReconciliation (ParcelID, AccountID)
	select ParcelID, AccountID
	from (select ParcelNumber, NewOwnerName
		  from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
		  where HasConflict = 1
		  group by ParcelNumber, NewOwnerName) v
	join dbo.Parcel p on p.ParcelNumber = v.ParcelNumber
	join dbo.Account a on a.AccountName = v.NewOwnerName

end