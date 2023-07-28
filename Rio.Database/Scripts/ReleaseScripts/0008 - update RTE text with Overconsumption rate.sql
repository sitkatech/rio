DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0008 - update RTE text with Overconsumption rate'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

	PRINT @MigrationName;

	update dbo.CustomRichText
  set CustomRichTextContent = REPLACE(CustomRichTextcontent COLLATE SQL_Latin1_General_CP1_CS_AS,'overconsumption rate','Water Charge Rate')
  where CustomRichTextTypeID = 35

  update dbo.CustomRichText
  set CustomRichTextContent = REPLACE(CustomRichTextcontent COLLATE SQL_Latin1_General_CP1_CS_AS,'Overconsumption Rate','Water Charge Rate')
  where CustomRichTextTypeID = 35

    update dbo.CustomRichText
  set CustomRichTextContent = REPLACE(CustomRichTextcontent COLLATE SQL_Latin1_General_CP1_CS_AS,'Overconsumption Volume','Water Charge Quantity')
  where CustomRichTextTypeID = 35

    update dbo.CustomRichText
  set CustomRichTextContent = REPLACE(CustomRichTextcontent COLLATE SQL_Latin1_General_CP1_CS_AS,'Overconsumption&nbsp;Amount','Estimated Water Charge Amount')
  where CustomRichTextTypeID = 35


    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Stewart Gordon', @MigrationName, '0008 - update RTE text with Overconsumption rate'
END