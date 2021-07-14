drop table dbo.OpenETSyncHistory

alter table dbo.WaterYear
drop column FinalizeDate

create table dbo.WaterYearMonth (
	WaterYearMonthID int identity(1,1) not null constraint PK_WaterYearMonth_WaterYearMonthID primary key,
	WaterYearID int not null constraint FK_WaterYearMonth_WaterYear_WaterYearID foreign key references dbo.WaterYear(WaterYearID),
	[Month] int not null,
	FinalizeDate datetime null,
	constraint AK_WaterYearMonth_WaterYearID_Month unique (WaterYearID, [Month])
)

CREATE TABLE dbo.OpenETSyncHistory(
	OpenETSyncHistoryID int IDENTITY(1,1) NOT NULL constraint PK_OpenETSyncHistory_OpenETSyncHistoryID primary key,
	WaterYearMonthID int NOT NULL constraint FK_OpenETSyncHistory_WaterYearMonth_WaterYearMonthID foreign key references dbo.WaterYearMonth (WaterYearMonthID),
	OpenETSyncResultTypeID int NOT NULL constraint FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID foreign key references dbo.OpenETSyncResultType (OpenETSyncResultTypeID),
	CreateDate datetime NOT NULL,
	UpdateDate datetime NOT NULL,
	GoogleBucketFileRetrievalURL varchar(200) NULL
)