CREATE TABLE dbo.PostingType(
    PostingTypeID int NOT NULL CONSTRAINT PK_PostingType_PostingTypeID PRIMARY KEY,
    PostingTypeName varchar(100) NOT NULL CONSTRAINT AK_PostingType_PostingTypeName UNIQUE,
    PostingTypeDisplayName varchar(100) NOT NULL CONSTRAINT AK_PostingType_PostingTypeDisplayName UNIQUE
)