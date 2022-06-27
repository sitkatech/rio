DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0003 - Insert ownership relationships between Accounts and Parcels for historic WaterYears'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;


	insert into dbo.AccountParcelWaterYear
	select apwy.AccountID, apwy.ParcelID, wyti.WaterYearID
	from dbo.AccountParcelWaterYear apwy
	cross join
	(
		SELECT wy.WaterYearID as WaterYearID
		FROM WaterYear wy
		  LEFT JOIN AccountParcelWaterYear apwy
		  ON apwy.WaterYearID = wy.WaterYearID
		WHERE apwy.WaterYearID IS NULL
	) wyti
	join dbo.WaterYear wy on apwy.WaterYearID = wy.WaterYearID
	where wy.[Year] = 2021


    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Andy Schultheiss', @MigrationName, '0003 - Insert ownership relationships between Accounts and Parcels for historic WaterYears'
END