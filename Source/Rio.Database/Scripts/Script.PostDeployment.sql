/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\LookupTables\Script.PostDeployment.LookupTables.sql
GO

:r .\Views\Script.PostDeployment.Views.sql
GO

:r .\Procs\Script.PostDeployment.Procs.sql
GO

:r .\ReleaseScripts\Script.PostDeployment.ReleaseScripts.sql
GO