CREATE TABLE dbo.SupportRequestLog(
    SupportRequestLogID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SupportRequestLog_SupportRequestLogID PRIMARY KEY,
    RequestDate datetime NOT NULL,
    RequestUserName varchar(200) NOT NULL,
    RequestUserEmail varchar(256) NOT NULL,
    RequestUserID int NULL CONSTRAINT FK_SupportRequestLog_User_RequestUserID_UserID FOREIGN KEY REFERENCES dbo.[User] (UserID),
    SupportRequestTypeID int NOT NULL CONSTRAINT FK_SupportRequestLog_SupportRequestType_SupportRequestTypeID FOREIGN KEY REFERENCES dbo.SupportRequestType (SupportRequestTypeID),
    RequestDescription varchar(2000) NOT NULL,
    RequestUserOrganization varchar(500) NULL,
    RequestUserPhone varchar(50) NULL,
)