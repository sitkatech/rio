create table dbo.OpenETSyncStatusType (
	OpenETSyncStatusTypeID int not null constraint PK_OpenETSyncStatusType_OpenETSyncStatusTypeID primary key,
	OpenETSyncStatusTypeName varchar(100) not null constraint AK_OpenETSyncStatusType_OpenETSyncStatusTypeName unique,
	OpenETSyncStatusTypeDisplayName varchar(100) not null constraint AK_OpenETSyncStatusType_OpenETSyncStatusTypeDisplayName unique
)

insert into dbo.OpenETSyncStatusType(OpenETSyncStatusTypeID, OpenETSyncStatusTypeName, OpenETSyncStatusTypeDisplayName)
values (1, 'Nightly', 'Nightly'),
(2, 'Finalized', 'Finalized'),
(3, 'CurrentlyUpdating', 'Current Updating')

create table dbo.OpenETSyncWaterYearStatus (
	OpenETSyncWaterYearStatusID int not null identity(1,1) constraint PK_OpenETSyncWaterYearStatus_OpenETSyncWaterYearStatusID primary key,
	WaterYear int not null constraint AK_OpenETSyncWaterYearStatus_WaterYear unique,
	OpenETSyncStatusTypeID int not null constraint FK_OpenETSyncWaterYearStatus_OpenETSyncStatusType_OpenETSyncStatusTypeID foreign key references dbo.OpenETSyncStatusType(OpenETSyncStatusTypeID),
	LastUpdatedDate datetime null
)

insert into dbo.OpenETSyncWaterYearStatus (WaterYear, OpenETSyncStatusTypeID)
select distinct(WaterYear) as WaterYear, 1
from dbo.ParcelMonthlyEvapotranspiration