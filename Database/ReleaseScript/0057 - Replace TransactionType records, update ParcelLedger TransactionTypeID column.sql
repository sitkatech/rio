insert into dbo.TransactionType (TransactionTypeID, TransactionTypeName, SortOrder)
values
(1, 'Supply', 10),
(2, 'Usage', 20)

alter table dbo.TransactionType
drop column SortOrder

go

-- Allocation, Trade - Purchase, Trade - Sale as Supply
update dbo.ParcelLedger
set TransactionTypeID = 1
where TransactionTypeID in (11, 20, 21)

-- Measured Usage, Measured Usage Correction, Manual Adjustment as Usage
update dbo.ParcelLedger
set TransactionTypeID = 2
where TransactionTypeID in (17, 18, 19)

go 

delete from dbo.TransactionType
where TransactionTypeID not in (1, 2)