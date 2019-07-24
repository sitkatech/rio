CREATE TABLE [dbo].[WaterTransfer]
(
    WaterTransferID INT NOT NULL identity(1, 1) constraint PK_WaterTransfer_WaterTransferID primary key,
    TransferDate datetime not null,
    AcreFeetTransferred decimal(10, 4) null,
    TransferringUserID int not null constraint FK_WaterTransfer_User_TransferringUserID_UserID foreign key references dbo.[User](UserID),
    ReceivingUserID int not null constraint FK_WaterTransfer_User_ReceivingUserID_UserID foreign key references dbo.[User](UserID),
    OfferID int null constraint FK_WaterTransfer_Offer_OfferID foreign key references dbo.Offer(OfferID),
    Notes varchar(2000) null
)
GO