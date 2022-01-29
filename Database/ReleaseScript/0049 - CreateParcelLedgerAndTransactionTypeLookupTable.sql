CREATE TABLE dbo.TransactionType
(
	TransactionTypeID int not null constraint PK_TransactionType_TransactionTypeID primary key,
	TransactionTypeName varchar (50) not null,
	SortOrder int not null 
)

CREATE TABLE dbo.ParcelLedger
(
	ParcelLedgerID int not null identity(1, 1) constraint PK_ParcelLedger_ParcelLedgerID primary key,
	ParcelID int not null constraint FK_ParcelLedger_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
	TransactionDate datetime not null,
	EffectiveDate datetime not null,
	TransactionTypeID int not null constraint FK_ParcelLedger_TransactionType_TransactionTypeID foreign key references dbo.TransactionType(TransactionTypeID),
	TransactionAmount decimal(10,4) not null,
	WaterTypeID int null constraint FK_ParcelLedger_WaterType_WaterTypeID foreign key references dbo.ParcelAllocationType(ParcelAllocationTypeID),
	TransactionDescription varchar (200) not null,
	UserID int null constraint FK_ParcelLedger_User_UserID foreign key references dbo.[User](UserID),
	UserComment varchar (max) null
	--, constraint AK_ParcelLedger_ParcelID_TransactionDate_TransactionTypeID_WaterTypeID unique(ParcelID, TransactionDate, TransactionTypeID, WaterTypeID)
)

go

insert into dbo.TransactionType (TransactionTypeID, TransactionTypeName, SortOrder)
values 
(11, 'Allocation', 10),
(17, 'Measured Usage', 70),
(18, 'Measured Usage Correction', 80),
(19, 'Manual Adjustment', 90),
(20, 'Trade - Purchase', 100),
(21, 'Trade - Sale', 110)
