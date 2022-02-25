CREATE TABLE [dbo].[FileResourceMimeType](
	[FileResourceMimeTypeID] [int] NOT NULL CONSTRAINT [PK_FileResourceMimeType_FileResourceMimeTypeID] PRIMARY KEY,
	[FileResourceMimeTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_FileResourceMimeType_FileResourceMimeTypeName] UNIQUE,
	[FileResourceMimeTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_FileResourceMimeType_FileResourceMimeTypeDisplayName] UNIQUE,
	[FileResourceMimeTypeContentTypeName] [varchar](100) NOT NULL,
	[FileResourceMimeTypeIconSmallFilename] [varchar](100) NULL,
	[FileResourceMimeTypeIconNormalFilename] [varchar](100) NULL
)
