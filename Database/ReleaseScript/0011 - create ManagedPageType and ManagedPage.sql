CREATE TYPE [dbo].[html] FROM [varchar](max) NULL
GO

CREATE TABLE [dbo].[CustomRichTextType](
	[CustomRichTextTypeID] [int] NOT NULL,
	[CustomRichTextTypeName] [varchar](100) NOT NULL,
	[CustomRichTextTypeDisplayName] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CustomRichTextType_CustomRichTextTypeID] PRIMARY KEY CLUSTERED 
(
	[CustomRichTextTypeID]
),
 CONSTRAINT [AK_CustomRichTextType_CustomRichTextTypeDisplayName] UNIQUE NONCLUSTERED 
(
	[CustomRichTextTypeDisplayName]
),
 CONSTRAINT [AK_CustomRichTextType_CustomRichTextTypeName] UNIQUE NONCLUSTERED 
(
	[CustomRichTextTypeName] ASC
)
)
GO

CREATE TABLE [dbo].[CustomRichText](
	[CustomRichTextID] [int] IDENTITY(1,1) NOT NULL,
	[CustomRichTextTypeID] [int] NOT NULL,
	[CustomRichTextContent] [dbo].[html] NULL,
 CONSTRAINT [PK_CustomRichText_CustomRichTextID] PRIMARY KEY CLUSTERED 
(
	[CustomRichTextID] ASC
)
)
GO

ALTER TABLE [dbo].[CustomRichText]  WITH CHECK ADD  CONSTRAINT [FK_CustomRichText_CustomRichTextType_CustomRichTextTypeID] FOREIGN KEY([CustomRichTextTypeID])
REFERENCES [dbo].[CustomRichTextType] ([CustomRichTextTypeID])
GO

ALTER TABLE [dbo].[CustomRichText] CHECK CONSTRAINT [FK_CustomRichText_CustomRichTextType_CustomRichTextTypeID]
GO

Insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values

(1, 'HomePage', 'Home Page'),
(2, 'Contact', 'Contact'),
(3, 'FrequentlyAskedQuestions', 'Frequently Asked Questons'),
(4, 'AboutGET', 'About GET'),
(5, 'Disclaimer', 'Disclaimer'),
(6, 'PlatformOverview', 'Platform Overview'),
(7, 'MeasuringWaterUse', 'Measureing Water Use With OpenET')

Insert into dbo.CustomRichText(CustomRichTextTypeID, CustomRichTextContent)
values
(1, 'Hompeage content'),
(2, 'Contact info'),
(3, 'FAQ'),
(4, 'About GET'),
(5, 'Disclaimer'),
(6, 'Platform Overview'),
(7, 'Measuring Water Use')