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

:r .\dbo.RioPageType.sql
GO

:r .\dbo.Role.sql 
GO

:r .\dbo.PostingType.sql 
GO

:r .\dbo.OfferStatus.sql 
GO

:r .\dbo.PostingStatus.sql 
GO

:r .\dbo.TradeStatus.sql 
GO

:r .\dbo.WaterTransferRegistrationStatus.sql 
GO

:r .\dbo.WaterTransferType.sql 
GO
