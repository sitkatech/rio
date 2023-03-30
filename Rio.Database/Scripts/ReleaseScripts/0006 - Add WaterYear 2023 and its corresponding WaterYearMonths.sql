DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0006 - Add WaterYear 2023 and its corresponding WaterYearMonths'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;


    insert into dbo.WaterYear([Year]) 
    select y.WaterYear from 
    (
	    select 2023 as WaterYear
    ) y 
    left join dbo.WaterYear wy on y.WaterYear = wy.[Year] 
    where wy.WaterYearID is null
    order by y.WaterYear

    insert into dbo.WaterYearMonth (WaterYearID, [Month])
    select wy.WaterYearID, m.MonthNumber
    from dbo.WaterYear wy
    cross join 
    (
        SELECT 1 as MonthNumber UNION 
        SELECT 2 as MonthNumber UNION 
        SELECT 3 as MonthNumber UNION 
        SELECT 4 as MonthNumber UNION 
        SELECT 5 as MonthNumber UNION 
        SELECT 6 as MonthNumber UNION 
        SELECT 7 as MonthNumber UNION 
        SELECT 8 as MonthNumber UNION 
        SELECT 9 as MonthNumber UNION 
        SELECT 10 as MonthNumber UNION 
        SELECT 11 as MonthNumber UNION 
        SELECT 12 as MonthNumber
    ) m
    left join dbo.WaterYearMonth wym on wy.WaterYearID = wym.WaterYearID and m.MonthNumber = wym.[Month]
    where wym.WaterYearMonthID is null
    order by wy.[Year], m.MonthNumber


    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Jamie Quishenberry', @MigrationName, 'Add WaterYear 2023 and its corresponding WaterYearMonths'
END