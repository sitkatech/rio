create table dbo.WaterTransferParcel
(
	WaterTransferParcelID int not null identity(1,1) constraint PK_WaterTransferParcel_WaterTransferParcelID primary key,
	WaterTransferID int not null constraint FK_WaterTransferParcel_WaterTransfer_WaterTransferID foreign key references dbo.WaterTransfer(WaterTransferID),
	ParcelID int not null constraint FK_WaterTransferParcel_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
	WaterTransferTypeID int not null  constraint FK_WaterTransferParcel_WaterTransferType_WaterTransferTypeID foreign key references dbo.WaterTransferType(WaterTransferTypeID),
    AcreFeetTransferred int not null
)