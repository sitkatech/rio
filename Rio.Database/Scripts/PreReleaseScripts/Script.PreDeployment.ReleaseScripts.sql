/*
Pre-Deployment Script
--------------------------------------------------------------------------------------
This file is generated on every build, DO NOT modify.
--------------------------------------------------------------------------------------
*/

PRINT N'Rio.Database - Script.PreDeployment.ReleaseScripts.sql';
GO

:r ".\001- Making TransactionType a lookup table.sql"
GO
:r ".\002- Inserting values to TransactionType.sql"
GO

