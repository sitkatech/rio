CREATE TABLE [dbo].[Offer]
(
    OfferID INT NOT NULL identity(1,1) constraint PK_Offer_OfferID primary key,
    TradeID int not null constraint FK_Offer_Trade_TradeID foreign key references dbo.Trade(TradeID),
    OfferDate datetime not null,
    Quantity int not null,
    Price money not null,
    OfferStatusID int not null constraint FK_Offer_OfferStatus_OfferStatusID foreign key references dbo.OfferStatus(OfferStatusID),
    OfferNotes varchar(2000) null,
    CreateUserID int not null constraint FK_Offer_User_CreateUserID_UserID foreign key references dbo.[User](UserID)
)
