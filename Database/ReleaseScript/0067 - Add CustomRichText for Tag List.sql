insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values (30, 'TagList', 'Tag List')

insert into dbo.CustomRichText (CustomRichTextTypeID, CustomRichTextContent)
values (30, '<p>A complete list of all parcel tags is provided here. Click a tag to see an index of each parcel the tag has been applied to. Deleting a tag from this list will remove the association from all tagged parcels.</p>')

