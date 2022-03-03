CREATE TABLE [dbo].[CustomRichTextType](
	[CustomRichTextTypeID] [int] NOT NULL CONSTRAINT [PK_CustomRichTextType_CustomRichTextTypeID] PRIMARY KEY,
	[CustomRichTextTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_CustomRichTextType_CustomRichTextTypeName] UNIQUE,
	[CustomRichTextTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_CustomRichTextType_CustomRichTextTypeDisplayName] UNIQUE,
)
