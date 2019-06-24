CREATE TABLE dbo.FieldDefinition(
    FieldDefinitionID int NOT NULL CONSTRAINT PK_FieldDefinition_FieldDefinitionID PRIMARY KEY,
    FieldDefinitionName varchar(300) NOT NULL CONSTRAINT AK_FieldDefinition_FieldDefinitionName UNIQUE,
    FieldDefinitionDisplayName varchar(300) NOT NULL CONSTRAINT AK_FieldDefinition_FieldDefinitionDisplayName UNIQUE,
    DefaultDefinition varchar(max) NOT NULL,
    CanCustomizeLabel bit NOT NULL
)