create table dbo.WaterTransferRegistration
(
    WaterTransferRegistrationID int not null identity(1,1) constraint PK_WaterTransferRegistration_WaterTransferRegistrationID primary key,
    WaterTransferID int not null constraint FK_WaterTransferRegistration_WaterTransfer_WaterTransferID foreign key references dbo.WaterTransfer(WaterTransferID),
    WaterTransferTypeID int not null constraint FK_WaterTransferRegistration_WaterTransferType_WaterTransferTypeID foreign key references dbo.WaterTransferType(WaterTransferTypeID),
    AccountID int not null constraint FK_WaterTransferRegistration_Account_AccountID foreign key references dbo.[Account](AccountID),
    WaterTransferRegistrationStatusID int NOT null constraint FK_WaterTransferRegistration_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusID foreign key references dbo.WaterTransferRegistrationStatus(WaterTransferRegistrationStatusID),
    StatusDate datetime NOT null
)
