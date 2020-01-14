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

/*
:r ".\0001 - Initial Data Population.sql"
GO

:r ".\0002 - Associating Users to Parcels.sql"
GO

:r ".\0003 - PMET_Import.sql"
GO

:r ".\0004 - Adding User Parcels part two.sql"
GO

:r ".\0005 - Associating Users with Accounts.sql"
GO */