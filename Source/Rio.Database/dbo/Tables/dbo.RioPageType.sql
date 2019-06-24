CREATE TABLE dbo.RioPageType(
    RioPageTypeID int NOT NULL CONSTRAINT PK_RioPageType_RioPageTypeID PRIMARY KEY,
    RioPageTypeName varchar(100) NOT NULL CONSTRAINT AK_RioPageType_RioPageTypeName UNIQUE,
    RioPageTypeDisplayName varchar(100) NOT NULL CONSTRAINT AK_RioPageType_RioPageTypeDisplayName UNIQUE
)