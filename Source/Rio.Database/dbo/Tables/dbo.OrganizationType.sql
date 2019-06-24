CREATE TABLE dbo.OrganizationType(
    OrganizationTypeID int NOT NULL CONSTRAINT PK_OrganizationType_OrganizationTypeID PRIMARY KEY,
    OrganizationTypeName varchar(200) NOT NULL,
    OrganizationTypeAbbreviation varchar(100) NOT NULL,
    LegendColor varchar(10) NOT NULL,
    IsDefaultOrganizationType bit NOT NULL,
    CONSTRAINT AK_OrganizationType_OrganizationTypeName UNIQUE (OrganizationTypeName)
)