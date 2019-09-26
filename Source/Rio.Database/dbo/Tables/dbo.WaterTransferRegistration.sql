create table dbo.WaterTransferRegistration
(
	WaterTransferRegistrationID int not null identity(1,1) constraint PK_WaterTransferRegistration_WaterTransferRegistrationID primary key,
	WaterTransferID int not null constraint FK_WaterTransferRegistration_WaterTransfer_WaterTransferID foreign key references dbo.WaterTransfer(WaterTransferID),
	WaterTransferTypeID int not null constraint FK_WaterTransferRegistration_WaterTransferType_WaterTransferTypeID foreign key references dbo.WaterTransferType(WaterTransferTypeID),
    UserID int not null constraint FK_WaterTransferRegistration_User_UserID foreign key references dbo.[User](UserID),
	DateRegistered datetime null
)