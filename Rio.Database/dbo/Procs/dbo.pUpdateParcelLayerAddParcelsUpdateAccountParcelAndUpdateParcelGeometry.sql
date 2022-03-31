create procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
(
    @waterYearID int
)
as

begin

	declare @squareFeetToAcresDivisor int = 43560

	--Function relies on ParcelGeometry being in EPSG:2229, or just a base geometry where the Projection's units are in feet
	insert into dbo.Parcel (ParcelNumber, ParcelGeometry, ParcelAreaInSquareFeet, ParcelAreaInAcres, ParcelStatusID, InactivateDate)
	select pus.ParcelNumber, 
		   pus.ParcelGeometry4326, 
		   round(pus.ParcelGeometry.STArea(), 0),
		   round(pus.ParcelGeometry.STArea() / @squareFeetToAcresDivisor, 14),
		   case when pus.HasConflict = 1 then 2 else 1 end,
		   case when pus.HasConflict = 1 then GETUTCDATE() else null end
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
	from (select ParcelNumber, OwnerName
		  from dbo.ParcelUpdateStaging
		  where HasConflict = 0) pus
	join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber
	join dbo.Account a on pus.OwnerName = a.AccountName
	cross join (select WaterYearID
						  from dbo.WaterYear
						  where [Year] >= (select [Year]
										  from dbo.WaterYear
										  where WaterYearID = @waterYearID)) wy
	
	update dbo.Parcel
	set ParcelGeometry = case when pus.ParcelGeometry4326 is not null then pus.ParcelGeometry4326 else p.ParcelGeometry end,
	ParcelAreaInSquareFeet = case when pus.ParcelGeometry is not null then round(pus.ParcelGeometry.STArea(), 0) else ParcelAreaInSquareFeet end,
	ParcelAreaInAcres = case when pus.ParcelGeometry is not null then round(pus.ParcelGeometry.STArea() / @squareFeetToAcresDivisor, 14) else ParcelAreaInAcres end,
	--if we had a conflict or no new owner, inactivate
	ParcelStatusID = case when pus.HasConflict = 1 or pus.OwnerName is null then 2 else 1 end,
	InactivateDate = case when (pus.HasConflict = 1 or pus.OwnerName is null) and InactivateDate is not null then InactivateDate
						  when (pus.HasConflict = 1 or pus.OwnerName is null) and InactivateDate is null then GETUTCDATE()
						  else null end
	from dbo.ParcelUpdateStaging pus
	join (select max(ParcelUpdateStagingID) ParcelUpdateStagingID
		  from dbo.ParcelUpdateStaging
		  group by ParcelNumber) pusu on pusu.ParcelUpdateStagingID = pus.ParcelUpdateStagingID
	right join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber

	insert into dbo.AccountReconciliation (ParcelID, AccountID)
	select ParcelID, AccountID
	from (select ParcelNumber, OwnerName
		  from dbo.ParcelUpdateStaging
		  where HasConflict = 1) pus
	join dbo.Parcel p on p.ParcelNumber = pus.ParcelNumber
	join dbo.Account a on a.AccountName = pus.OwnerName


end