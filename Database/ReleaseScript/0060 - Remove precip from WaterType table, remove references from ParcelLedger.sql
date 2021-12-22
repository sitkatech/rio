update dbo.ParcelLedger
set WaterTypeID = null
where WaterTypeID = 5

go

delete from dbo.WaterType
where WaterTypeID = 5