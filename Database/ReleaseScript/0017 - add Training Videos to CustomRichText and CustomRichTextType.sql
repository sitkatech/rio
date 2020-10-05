Insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values
(10, 'TrainingVideos', 'Training Videos')

Insert into dbo.CustomRichText(CustomRichTextTypeID, CustomRichTextContent)
values
(10, 'Training Videos default content')