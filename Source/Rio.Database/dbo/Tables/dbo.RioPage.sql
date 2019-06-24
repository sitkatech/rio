CREATE TABLE dbo.RioPage(
    RioPageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RioPage_RioPageID PRIMARY KEY,
    RioPageTypeID int NOT NULL,
    RioPageContent varchar(max) NULL,
    CONSTRAINT AK_RioPage_RioPageTypeID UNIQUE (RioPageTypeID)
)
