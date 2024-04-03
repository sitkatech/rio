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
:r ".\0003 - Insert ownership relationships between Accounts and Parcels for historic WaterYears.sql"
GO
:r ".\0004 - Add RTE for Usage CSV Upload.sql"
GO
:r ".\0005 - Add RTE for Overconsumption Rate editor.sql"
GO
:r ".\0006 - Add WaterYear 2023 and its corresponding WaterYearMonths.sql"
GO
:r ".\0007 - Create 2023 ownership relationships.sql"
GO
:r ".\0008 - update RTE text with Overconsumption rate.sql"
GO
:r ".\0009 - Add 2024 WaterYear record.sql"
GO
:r ".\0010 - Create 2024 ownership relationships.sql"
GO

