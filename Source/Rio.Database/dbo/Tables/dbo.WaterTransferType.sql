create table dbo.WaterTransferType
(
    WaterTransferTypeID int not null constraint PK_WaterTransferType_WaterTransferTypeID primary key,
    WaterTransferTypeName varchar(50) not null constraint AK_WaterTransferType_WaterTransferTypeName unique,
    WaterTransferTypeDisplayName varchar(50) not null constraint AK_WaterTransferType_WaterTransferTypeDisplayName unique
)