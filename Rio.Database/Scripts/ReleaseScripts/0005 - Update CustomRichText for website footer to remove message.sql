DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0005 - Update CustomRichText for website footer to remove message'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;

    update dbo.CustomRichText
    set CustomRichTextContent = '<figure class="table"><table><tbody><tr><td>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</td><td><p>&nbsp;</p><figure class="image"><img src="https://api-rio-rrb.aks-prod.sitkatech.com/FileResource/b892427b-fc14-477c-aef6-9fc9146fdd3c"></figure></td><td>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</td><td><p>&nbsp;</p><p><strong>Rosedale-Rio Bravo Water Storage District</strong><br>Physical/Mailing Address:<br>849 Allen Road<br>Bakersfield, CA 93314<br>Phone: 661.589.6045<br>FAX: 661.589.1867</p></td></tr></tbody></table></figure>'
    where CustomRichTextTypeID = 26
    
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Liz Arikawa', @MigrationName, '0005 - Update CustomRichText for website footer to remove message'
END