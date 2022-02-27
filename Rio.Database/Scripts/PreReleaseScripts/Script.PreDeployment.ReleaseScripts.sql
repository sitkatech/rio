/*
Pre-Deployment Script
--------------------------------------------------------------------------------------
This file is generated on every build, DO NOT modify.
--------------------------------------------------------------------------------------
*/

PRINT N'Rio.Database - Script.PreDeployment.ReleaseScripts.sql';

delete
from dbo.CustomRichText
where CustomRichTextTypeID = 27

GO


