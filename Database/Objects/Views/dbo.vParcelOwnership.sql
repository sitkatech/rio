if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelOwnership'))
	drop view dbo.vParcelOwnership
go

create view dbo.vParcelOwnership
as

select          allParcelsForAllWaterYears.ParcelID,
				allParcelsForAllWaterYears.WaterYearID,
				apwy.AccountID
                
FROM        (select wy.WaterYearID, p.ParcelID
			 from dbo.WaterYear wy
			 cross join dbo.Parcel p) allParcelsForAllWaterYears
LEFT JOIN dbo.AccountParcelWaterYear apwy on apwy.ParcelID = allParcelsForAllWaterYears.ParcelID and apwy.WaterYearID = allParcelsForAllWaterYears.WaterYearID


GO