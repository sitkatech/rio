create table dbo.ParcelAllocationType
(
    ParcelAllocationTypeID int not null constraint PK_ParcelAllocationType_ParcelAllocationTypeID primary key,
    ParcelAllocationTypeName varchar(50) not null constraint AK_ParcelAllocationType_ParcelAllocationTypeName unique,
    ParcelAllocationTypeDisplayName varchar(50) not null constraint AK_ParcelAllocationType_ParcelAllocationTypeDisplayName unique
)