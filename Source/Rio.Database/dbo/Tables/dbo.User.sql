CREATE TABLE dbo.[User](
    UserID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_User_UserID PRIMARY KEY,
    UserGuid uniqueidentifier NOT NULL,
    FirstName varchar(100) NOT NULL,
    LastName varchar(100) NOT NULL,
    Email varchar(255) NOT NULL,
    Phone varchar(30) NULL,
    RoleID int NOT NULL CONSTRAINT FK_User_Role_RoleID FOREIGN KEY REFERENCES dbo.Role (RoleID),
    CreateDate datetime NOT NULL,
    UpdateDate datetime NULL,
    LastActivityDate datetime NULL,
    IsActive bit NOT NULL,
    OrganizationID int NULL CONSTRAINT FK_User_Organization_OrganizationID FOREIGN KEY REFERENCES dbo.Organization (OrganizationID),
    ReceiveSupportEmails bit NOT NULL,
    LoginName varchar(128) NOT NULL,
    CONSTRAINT AK_User_Email UNIQUE (Email),
    CONSTRAINT AK_User_UserGuid UNIQUE (UserGuid)
)
GO

CREATE UNIQUE INDEX AK_Organization_OrganizationGuid ON dbo.Organization
(
    OrganizationGuid
)
WHERE (OrganizationGuid IS NOT NULL)