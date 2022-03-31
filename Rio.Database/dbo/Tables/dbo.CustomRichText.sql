CREATE TABLE [dbo].[CustomRichText](
	[CustomRichTextID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomRichText_CustomRichTextID] PRIMARY KEY,
	[CustomRichTextTypeID] [int] NOT NULL CONSTRAINT [FK_CustomRichText_CustomRichTextType_CustomRichTextTypeID] FOREIGN KEY REFERENCES [dbo].[CustomRichTextType] ([CustomRichTextTypeID]),
	[CustomRichTextContent] [dbo].[html] NULL,
)