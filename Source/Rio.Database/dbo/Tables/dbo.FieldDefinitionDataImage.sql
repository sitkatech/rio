CREATE TABLE dbo.FieldDefinitionDataImage(
    FieldDefinitionDataImageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldDefinitionDataImage_FieldDefinitionDataImageID PRIMARY KEY,
    FieldDefinitionDataID int NOT NULL CONSTRAINT FK_FieldDefinitionDataImage_FieldDefinitionData_FieldDefinitionDataID FOREIGN KEY REFERENCES dbo.FieldDefinitionData (FieldDefinitionDataID),
    FileResourceID int NOT NULL CONSTRAINT FK_FieldDefinitionDataImage_FileResource_FileResourceID FOREIGN KEY REFERENCES dbo.FileResource (FileResourceID)
)