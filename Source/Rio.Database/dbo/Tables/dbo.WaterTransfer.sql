CREATE TABLE [dbo].[WaterTransfer]
(
    WaterTransferID INT NOT NULL identity(1, 1) constraint PK_WaterTransfer_WaterTransferID primary key,
    TransferDate datetime not null,
    AcreFeetTransferred int NOT null,
    OfferID int not null constraint FK_WaterTransfer_Offer_OfferID foreign key references dbo.Offer(OfferID),
    Notes varchar(2000) null
)
GO