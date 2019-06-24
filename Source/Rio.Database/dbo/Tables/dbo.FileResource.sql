CREATE TABLE dbo.FileResource(
    FileResourceID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FileResource_FileResourceID PRIMARY KEY,
    FileResourceMimeTypeID int NOT NULL CONSTRAINT FK_FileResource_FileResourceMimeType_FileResourceMimeTypeID FOREIGN KEY REFERENCES dbo.FileResourceMimeType (FileResourceMimeTypeID),
    OriginalBaseFilename varchar(255) NOT NULL,
    OriginalFileExtension varchar(255) NOT NULL,
    FileResourceGUID uniqueidentifier NOT NULL CONSTRAINT AK_FileResource_FileResourceGUID UNIQUE,
    FileResourceData varbinary(max) NOT NULL,
    CreateUserID int NOT NULL CONSTRAINT FK_FileResource_User_CreateUserID_UserID FOREIGN KEY REFERENCES dbo.[User] (UserID),
    CreateDate datetime NOT NULL
)