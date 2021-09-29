CREATE TABLE dbo.TransactionType
(
	TransactionTypeID int not null constraint PK_TransactionType_TransactionTypeID primary key,
	TransactionTypeName varchar (50) not null
)

CREATE TABLE dbo.ParcelLedger
(
	ParcelLedgerID int not null identity(1, 1) constraint PK_ParcelLedger_ParcelLedgerID primary key,
	ParcelID int not null constraint FK_ParcelLedger_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
	TransactionDate datetime not null,
	TransactionTypeID int not null constraint FK_ParcelLedger_TransactionType_TransactionTypeID foreign key references dbo.TransactionType(TransactionTypeID),
	TransactionAmount float not null,
	TransactionDescription varchar (100) not null,
	constraint AK_ParcelLedger_ParcelID_TransactionDate_TransactionTypeID unique(ParcelID, TransactionDate, TransactionTypeID)
)

go

insert into dbo.TransactionType (TransactionTypeID, TransactionTypeName)
values 
(1, 'Allocation - Project Water'),
(2, 'Allocation - Reconciliation'),
(3, 'Allocation - Native Yield'),
(4, 'Allocation - Stored Water'),
(5, 'Allocation - Precipitation'),
(6, 'Allocation - Allowable Imbalance'),
(7, 'Measured Usage'),
(8, 'Measured Usage Correction'),
(9, 'Manual Adjustment'),
(10, 'Trade - Purchase'),
(11, 'Trade - Sale')
