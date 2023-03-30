CREATE TABLE [dbo].[ParcelUsageFileUpload] (
	[ParcelUsageFileUploadID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelUsageFileUpload_ParcelUsageFileUploadID] PRIMARY KEY,
	[UserID] [int] NOT NULL CONSTRAINT [FK_ParcelUsageFileUpload_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[UploadedFileName] [varchar](100) NOT NULL,
	[UploadDate] [datetime] NOT NULL,
	[PublishDate] [datetime] NULL,
	[MatchedRecordCount] [int] NOT NULL,
	[UnmatchedParcelNumberCount] [int] NOT NULL,
	[NullParcelNumberCount] [int] NOT NULL
)
