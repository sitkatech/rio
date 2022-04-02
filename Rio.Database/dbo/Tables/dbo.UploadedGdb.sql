CREATE TABLE [dbo].[UploadedGdb](
	[UploadedGdbID] [int] NOT NULL CONSTRAINT [PK_UploadedGdb_UploadedGdbID] PRIMARY KEY,
	[GdbFileContents] [varbinary](max) NULL,
	[UploadDate] [datetime] NOT NULL
)
