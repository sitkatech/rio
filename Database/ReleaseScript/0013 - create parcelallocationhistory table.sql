create table dbo.ParcelAllocationHistory (
	ParcelAllocationHistoryID int not null identity(1,1) constraint PK_ParcelAllocationHistory_ParcelAllocationHistoryID primary key,
	ParcelAllocationHistoryDate Datetime not null,
	ParcelAllocationHistoryWaterYear int not null,
	ParcelAllocationTypeID int not null constraint FK_ParcelAllocationHistory_ParcelAllocationType_ParcelAllocationTypeID foreign key references dbo.ParcelAllocationType(ParcelAllocationTypeID),
	UserID int not null constraint FK_ParcelAllocationHistory_User_UserID foreign key references dbo.[User] (UserID),
	FileResourceID int null constraint FK_ParcelAllocationHistory_FileResource_FileResourceID foreign key references dbo.FileResource (FileResourceID),
	ParcelAllocationHistoryValue decimal(10,4) null
)