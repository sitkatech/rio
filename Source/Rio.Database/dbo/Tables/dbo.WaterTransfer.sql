CREATE TABLE [dbo].[WaterTransfer]
(
    WaterTransferID INT NOT NULL identity(1, 1) constraint PK_WaterTransfer_WaterTransferID primary key,
    TransferDate datetime not null,
    AcreFeetTransferred int NOT null,
    TransferringUserID int not null constraint FK_WaterTransfer_User_TransferringUserID_UserID foreign key references dbo.[User](UserID),
    ReceivingUserID int not null constraint FK_WaterTransfer_User_ReceivingUserID_UserID foreign key references dbo.[User](UserID),
    OfferID int not null constraint FK_WaterTransfer_Offer_OfferID foreign key references dbo.Offer(OfferID),
    Notes varchar(2000) null,
    [ConfirmedByTransferringUser] bit not null,
    [DateConfirmedByTransferringUser] datetime null,
    [ConfirmedByReceivingUser] bit not null,
    [DateConfirmedByReceivingUser] datetime null
)
GO