SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomRichText](
	[CustomRichTextID] [int] IDENTITY(1,1) NOT NULL,
	[CustomRichTextTypeID] [int] NOT NULL,
	[CustomRichTextContent] [dbo].[html] NULL,
 CONSTRAINT [PK_CustomRichText_CustomRichTextID] PRIMARY KEY CLUSTERED 
(
	[CustomRichTextID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[CustomRichText]  WITH CHECK ADD  CONSTRAINT [FK_CustomRichText_CustomRichTextType_CustomRichTextTypeID] FOREIGN KEY([CustomRichTextTypeID])
REFERENCES [dbo].[CustomRichTextType] ([CustomRichTextTypeID])
GO
ALTER TABLE [dbo].[CustomRichText] CHECK CONSTRAINT [FK_CustomRichText_CustomRichTextType_CustomRichTextTypeID]