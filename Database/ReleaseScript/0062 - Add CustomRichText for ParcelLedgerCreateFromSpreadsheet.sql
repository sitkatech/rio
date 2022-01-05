insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values (25, 'ParcelLedgerCreateFromSpreadsheet', 'Create Transaction from Spreadsheet Upload')

insert into dbo.CustomRichText (CustomRichTextTypeID, CustomRichTextContent)
values (25, 'Here is some info about creating a transaction from a spreadsheet upload...')