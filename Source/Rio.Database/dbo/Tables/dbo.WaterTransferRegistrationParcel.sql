create table dbo.WaterTransferRegistrationParcel
(
    WaterTransferRegistrationParcelID int not null identity(1,1) constraint PK_WaterTransferRegistrationParcel_WaterTransferRegistrationParcelID primary key,
    WaterTransferRegistrationID int not null constraint FK_WaterTransferRegistrationParcel_WaterTransferRegistration_WaterTransferRegistrationID foreign key references dbo.WaterTransferRegistration(WaterTransferRegistrationID),
    ParcelID int not null constraint FK_WaterTransferRegistrationParcel_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
    AcreFeetTransferred int not null
)