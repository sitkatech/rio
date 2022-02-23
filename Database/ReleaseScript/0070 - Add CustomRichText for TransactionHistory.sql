insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values (32, 'TransactionHistory', 'Transaction History')

insert into dbo.CustomRichText (CustomRichTextTypeID, CustomRichTextContent)
values (32, '<p>This grid displays a history of transactions created using either the Bulk Transaction or Spreadsheet Upload form.</p>')

