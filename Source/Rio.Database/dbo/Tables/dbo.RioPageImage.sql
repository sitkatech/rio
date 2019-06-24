CREATE TABLE dbo.RioPageImage(
    RioPageImageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RioPageImage_RioPageImageID PRIMARY KEY,
    RioPageID int NOT NULL CONSTRAINT FK_RioPageImage_RioPage_RioPageID FOREIGN KEY REFERENCES dbo.RioPage (RioPageID),
    FileResourceID int NOT NULL CONSTRAINT FK_RioPageImage_FileResource_FileResourceID FOREIGN KEY REFERENCES dbo.FileResource (FileResourceID),
    CONSTRAINT AK_RioPageImage_RioPageImageID_FileResourceID UNIQUE (RioPageImageID, FileResourceID)
)