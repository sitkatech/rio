CREATE TABLE dbo.FieldDefinitionData(
    FieldDefinitionDataID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldDefinitionData_FieldDefinitionDataID PRIMARY KEY,
    FieldDefinitionID int NOT NULL CONSTRAINT FK_FieldDefinitionData_FieldDefinition_FieldDefinitionID FOREIGN KEY REFERENCES dbo.FieldDefinition (FieldDefinitionID),
    FieldDefinitionDataValue varchar(max) NULL,
    FieldDefinitionLabel varchar(300) NULL,
    CONSTRAINT AK_FieldDefinitionData_FieldDefinitionID UNIQUE (FieldDefinitionID)
)