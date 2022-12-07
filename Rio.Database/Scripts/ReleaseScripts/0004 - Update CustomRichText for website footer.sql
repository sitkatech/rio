DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0004 - Update CustomRichText for website footer'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;

    update dbo.CustomRichText
    set CustomRichTextContent = '<figure class="table"><table><tbody><tr><td>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</td><td><p>&nbsp;</p><figure class="image"><img src="https://api-rio-rrb.aks-prod.sitkatech.com/FileResource/b892427b-fc14-477c-aef6-9fc9146fdd3c"></figure></td><td>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</td><td><p>&nbsp;</p><p><strong>Rosedale-Rio Bravo Water Storage District</strong><br>Physical/Mailing Address:<br>849 Allen Road<br>Bakersfield, CA 93314<br>Phone: 661.589.6045<br>FAX: 661.589.1867</p></td></tr></tbody></table></figure><div class="row mt-3"><div class="col-12 text-center"><p>The Groundwater Accounting Platform is developed using open-source software under the <a href="https://www.gnu.org/licenses/agpl-3.0.en.html">GNU Affero General Public License</a> (AGPL). It can be redistributed and/or modified under the terms of AGPL. Source code is available on <a href="https://github.com/sitkatech/rio">GitHub</a>.</p></div></div>'
    where CustomRichTextTypeID = 26
    
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Liz Arikawa', @MigrationName, '0004 - Update CustomRichText for website footer'
END