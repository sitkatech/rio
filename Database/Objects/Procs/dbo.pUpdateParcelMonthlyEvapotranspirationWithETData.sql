IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelMonthlyEvapotranspirationWithETData'))
    drop procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
go

create procedure dbo.pUpdateParcelMonthlyEvapotranspirationWithETData
as

begin
	MERGE INTO dbo.ParcelMonthlyEvapotranspiration AS Target
	USING dbo.ParcelMonthlyEvapotranspiration AS Source
	ON Target.ParcelID = Source.ParcelID and Target.WaterYear = Source.WaterYear and Target.WaterMonth = Source.WaterMonth
	WHEN MATCHED THEN
	UPDATE SET
		--Need to convert mm into Acre-Feet
		Target.EvapotranspirationRate = Source.EvapotranspirationRate
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (ParcelID, WaterYear, WaterMonth, EvapotranspirationRate)
		VALUES (Source.ParcelID, Source.WaterYear, Source.WaterMonth, Source.EvapotranspirationRate);
end