IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelMonthlyEvapotranspirationWithETData'))
    drop procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
go

create procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
as

begin
	MERGE INTO dbo.ParcelMonthlyEvapotranspiration AS Target
	USING (select p.ParcelID, p.ParcelAreaInAcres, et.WaterYear, et.WaterMonth, et.EvapotranspirationRateInMM
		   from dbo.Parcel p
		   join dbo.OpenETGoogleBucketResponseEvapotranspirationData et
		   on p.ParcelNumber = et.ParcelNumber) AS Source
	ON Target.ParcelID = Source.ParcelID and Target.WaterYear = Source.WaterYear and Target.WaterMonth = Source.WaterMonth
	WHEN MATCHED THEN
	UPDATE SET
		Target.EvapotranspirationRate = ((Source.EvapotranspirationRateInMM / 25.4) / 12) * Source.ParcelAreaInAcres
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (ParcelID, WaterYear, WaterMonth, EvapotranspirationRate)
		VALUES (Source.ParcelID, Source.WaterYear, Source.WaterMonth, ((Source.EvapotranspirationRateInMM / 25.4) / 12) * Source.ParcelAreaInAcres);
end