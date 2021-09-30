CREATE TABLE dbo.TransactionType
(
	TransactionTypeID int not null constraint PK_TransactionType_TransactionTypeID primary key,
	TransactionTypeName varchar (50) not null,
	IsAllocation bit not null,
	SortOrder int not null 
)

CREATE TABLE dbo.ParcelLedger
(
	ParcelLedgerID int not null identity(1, 1) constraint PK_ParcelLedger_ParcelLedgerID primary key,
	ParcelID int not null constraint FK_ParcelLedger_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
	TransactionDate datetime not null,
	TransactionTypeID int not null constraint FK_ParcelLedger_TransactionType_TransactionTypeID foreign key references dbo.TransactionType(TransactionTypeID),
	TransactionAmount decimal(10,4) not null,
	TransactionDescription varchar (200) not null,
	constraint AK_ParcelLedger_ParcelID_TransactionDate_TransactionTypeID unique(ParcelID, TransactionDate, TransactionTypeID)
)

go

insert into dbo.TransactionType (TransactionTypeID, TransactionTypeName, IsAllocation, SortOrder)
values 
(11, 'Project Water', 1, 10),
(12, 'Reconciliation', 1, 60),
(13, 'Native Yield', 1, 20),
(14, 'Stored Water', 1, 30),
(15, 'Precipitation', 1, 40),
(16, 'Allowable Imbalance', 1, 50),
(17, 'Measured Usage', 0, 70),
(18, 'Measured Usage Correction', 0, 80),
(19, 'Manual Adjustment', 0, 90),
(20, 'Trade - Purchase', 0, 100),
(21, 'Trade - Sale', 0, 110)
