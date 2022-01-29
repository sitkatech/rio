create table dbo.ParcelLedgerEntrySourceType
(
	ParcelLedgerEntrySourceTypeID int not null constraint PK_ParcelLedgerEntrySourceType_SourceTypeID primary key,
	ParcelLedgerEntrySourceTypeName varchar (50) not null constraint AK_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeName unique,
	ParcelLedgerEntrySourceTypeDisplayName varchar (50) not null constraint AK_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeDisplayName unique
)

go 

insert into dbo.ParcelLedgerEntrySourceType (ParcelLedgerEntrySourceTypeID, ParcelLedgerEntrySourceTypeName, ParcelLedgerEntrySourceTypeDisplayName)
values
(1, 'Manual', 'Manual'),
(2, 'OpenET', 'OpenET'),
(3, 'CIMIS', 'CIMIS'),
(4, 'Trade', 'Trade')

go 

alter table dbo.ParcelLedger add ParcelLedgerEntrySourceTypeID int null constraint FK_ParcelLedger_ParcelLedgerEntrySourceType_ParcelLedgerEntrySourceTypeID
foreign key (ParcelLedgerEntrySourceTypeID) references dbo.ParcelLedgerEntrySourceType(ParcelLedgerEntrySourceTypeID)

go 

update dbo.ParcelLedger set ParcelLedgerEntrySourceTypeID = 1

alter table dbo.ParcelLedger alter column ParcelLedgerEntrySourceTypeID int not null
GO

-- Allocation, Manual Adjustment as Manual
update dbo.ParcelLedger
set ParcelLedgerEntrySourceTypeID = 1
where TransactionTypeID = 11 or TransactionTypeID = 19

-- Allocation of Precipitation as CIMIS
update dbo.ParcelLedger
set ParcelLedgerEntrySourceTypeID = 3
where WaterTypeID = 5

-- Measured Usage, Measured Usage Correction as OpenET
update dbo.ParcelLedger
set ParcelLedgerEntrySourceTypeID = 2
where TransactionTypeID = 17 or TransactionTypeID = 18

-- Trade - Purchase, Trade - Sale as Trade
update dbo.ParcelLedger
set ParcelLedgerEntrySourceTypeID = 4
where TransactionTypeID = 20 or TransactionTypeID = 21