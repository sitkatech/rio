create table dbo.OpenETSyncResultType (
	OpenETSyncResultTypeID int not null constraint PK_OpenETSyncResultType_OpenETSyncResultTypeID primary key,
	OpenETSyncResultTypeName varchar(100) not null constraint AK_OpenETSyncResultType_AK_OpenETSyncResultTypeName unique,
	OpenETSyncResultTypeDisplayName varchar(100) not null constraint AK_OpenETSyncResultType_OpenETSyncResultTypeDisplayName unique
)

insert into dbo.OpenETSyncResultType (OpenETSyncResultTypeID, OpenETSyncResultTypeName, OpenETSyncResultTypeDisplayName)
values (1, 'InProgress', 'In Progress'),
(2, 'Succeeded', 'Succeeded'),
(3, 'Failed', 'Failed')

create table dbo.OpenETSyncHistory (
	OpenETSyncHistoryID int not null identity(1,1) constraint PK_OpenETSyncHistory_OpenETSyncHistoryID primary key,
	OpenETSyncResultTypeID int not null constraint FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID foreign key references dbo.OpenETSyncResultType(OpenETSyncResultTypeID),
	YearsInUpdateSeparatedByComma varchar(100) not null,
	UpdatedFileSuffix varchar(20) not null,
	LastUpdatedDate DateTime not null
) 