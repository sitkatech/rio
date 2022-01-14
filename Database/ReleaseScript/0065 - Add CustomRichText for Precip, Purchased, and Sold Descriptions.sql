insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values 
	(27, 'PrecipitationDescription', 'Precipitation Description'), 
	(28, 'PurchasedDescription', 'Purchased Water Description'), 
	(29, 'SoldDescription', 'Sold Water Description')

insert into dbo.CustomRichText (CustomRichTextTypeID, CustomRichTextContent)
values 
	(27, 'Here is some info about precipitation...'), 
	(28, 'Here is some info about purchased water...'), 
	(29, 'Here is some info about sold water...')