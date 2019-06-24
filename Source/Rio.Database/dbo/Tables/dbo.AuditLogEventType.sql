CREATE TABLE dbo.AuditLogEventType(
    AuditLogEventTypeID int NOT NULL CONSTRAINT PK_AuditLogEventType_AuditLogEventTypeID PRIMARY KEY,
    AuditLogEventTypeName varchar(100) NOT NULL CONSTRAINT AK_AuditLogEventType_AuditLogEventTypeName UNIQUE,
    AuditLogEventTypeDisplayName varchar(100) NOT NULL CONSTRAINT AK_AuditLogEventType_AuditLogEventTypeDisplayName UNIQUE 
)
