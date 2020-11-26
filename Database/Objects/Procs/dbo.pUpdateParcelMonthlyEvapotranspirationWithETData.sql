IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelMonthlyEvapotranspirationWithETData'))
    drop procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
go

create procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
as

begin
	MERGE INTO dbo.ParcelMonthlyEvapotranspiration AS Target
	USING (select p.ParcelID, oet.WaterYear, oet.WaterMonth, oet.EvapotranspirationRate
		   from dbo.OpenETGoogleBucketResponseEvapotranspirationData oet
		   join dbo.Parcel p on p.ParcelNumber = oet.ParcelNumber) AS Source
	ON Target.ParcelID = Source.ParcelID and Target.WaterYear = Source.WaterYear and Target.WaterMonth = Source.WaterMonth
	WHEN MATCHED THEN
	UPDATE SET
		Target.EvapotranspirationRate = Source.EvapotranspirationRate
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (ParcelID, WaterYear, WaterMonth, EvapotranspirationRate)
		VALUES (Source.ParcelID, Source.WaterYear, Source.WaterMonth, Source.EvapotranspirationRate);
end