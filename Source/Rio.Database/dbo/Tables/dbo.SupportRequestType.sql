CREATE TABLE dbo.SupportRequestType(
    SupportRequestTypeID int NOT NULL CONSTRAINT PK_SupportRequestType_SupportRequestTypeID PRIMARY KEY,
    SupportRequestTypeName varchar(100) NOT NULL CONSTRAINT AK_SupportRequestType_SupportRequestTypeName UNIQUE,
    SupportRequestTypeDisplayName varchar(100) NOT NULL CONSTRAINT AK_SupportRequestType_SupportRequestTypeDisplayName UNIQUE,
    SupportRequestTypeSortOrder int NOT NULL,
)