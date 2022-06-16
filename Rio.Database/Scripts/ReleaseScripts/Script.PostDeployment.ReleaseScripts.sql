/*
Post-Deployment Script
--------------------------------------------------------------------------------------
This file is generated on every build, DO NOT modify.
--------------------------------------------------------------------------------------
*/

PRINT N'Rio.Database - Script.PostDeployment.ReleaseScripts.sql';
GO

:r ".\0001 - Initial data script.sql"
GO
:r ".\0002 - Add WaterYears 2018-2022 and their corresponding WaterYearMonths.sql"
GO

