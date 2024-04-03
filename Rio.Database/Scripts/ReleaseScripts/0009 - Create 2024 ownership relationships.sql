DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0007 - Create 2024 ownership relationships'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;

	insert into dbo.AccountParcelWaterYear
	select apwy.AccountID, apwy.ParcelID, wyti.WaterYearID
	from dbo.AccountParcelWaterYear apwy
	join dbo.WaterYear wy on apwy.WaterYearID = wy.WaterYearID
	cross join
	(
		SELECT wy.WaterYearID as WaterYearID
		FROM WaterYear wy
		  LEFT JOIN AccountParcelWaterYear apwy
		  ON apwy.WaterYearID = wy.WaterYearID
		WHERE apwy.WaterYearID IS NULL
	) wyti
	where wy.[Year] = 2023


    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Jamie Quishenberry', @MigrationName, 'Create 2024 ownership relationships'
END