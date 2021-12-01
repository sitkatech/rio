create table dbo.ParcelLedgerEntrySourceType
(
	ParcelLedgerEntrySourceTypeID int not null constraint PK_ParcelLedgerEntrySourceType_SourceTypeID primary key,
	ParcelLedgerEntrySourceTypeName varchar (50) not null
)

go 

insert into dbo.ParcelLedgerEntrySourceType (ParcelLedgerEntrySourceTypeID, ParcelLedgerEntrySourceTypeName)
values
(1, 'Manual'),
(2, 'OpenET'),
(3, 'CIMIS'),
(4, 'Trade')

go 

alter table dbo.ParcelLedger
add ParcelLedgerEntrySourceTypeID int not null constraint FK_ParcelLedger_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeID default 1
foreign key (ParcelLedgerEntrySourceTypeID) references dbo.ParcelLedgerEntrySourceType(ParcelLedgerEntrySourceTypeID)

go 

-- Allocation, Manual Adjustment as Manual
update dbo.ParcelLedger
set ParcelLedgerEntrySourceTypeID = 1
where TransactionTypeID = 11 or TransactionTypeID = 19

-- Measured Usage, Measured Usage Correction as OpenET
update dbo.ParcelLedger
set ParcelLedgerEntrySourceTypeID = 2
where TransactionTypeID = 17 or TransactionTypeID = 18

-- Trade - Purchase, Trade - Sale as Trade
update dbo.ParcelLedger
set ParcelLedgerEntrySourceTypeID = 4
where TransactionTypeID = 20 or TransactionTypeID = 21