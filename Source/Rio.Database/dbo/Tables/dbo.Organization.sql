CREATE TABLE dbo.Organization(
    OrganizationID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Organization_OrganizationID PRIMARY KEY,
    OrganizationGuid uniqueidentifier NULL,
    OrganizationName varchar(200) NOT NULL CONSTRAINT AK_Organization_OrganizationName UNIQUE (OrganizationName),
    OrganizationShortName varchar(50) NULL,
    PrimaryContactUserID int NULL CONSTRAINT FK_Organization_User_PrimaryContactUserID_UserID FOREIGN KEY REFERENCES dbo.[User] (UserID),
    IsActive bit NOT NULL,
    OrganizationUrl varchar(200) NULL,
    LogoFileResourceID int NULL CONSTRAINT FK_Organization_FileResource_LogoFileResourceID_FileResourceID FOREIGN KEY REFERENCES dbo.FileResource (FileResourceID),
    OrganizationTypeID int NOT NULL CONSTRAINT FK_Organization_OrganizationType_OrganizationTypeID FOREIGN KEY REFERENCES dbo.OrganizationType (OrganizationTypeID)
)