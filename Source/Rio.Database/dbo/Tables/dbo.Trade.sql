CREATE TABLE [dbo].[Trade]
(
    TradeID INT NOT NULL identity(1,1) constraint PK_Trade_TradeID primary key,
    PostingID int not null constraint FK_Trade_Posting_PostingID foreign key references dbo.Posting(PostingID),
    TradeDate datetime not null,
    TradeStatusID int not null  constraint FK_Trade_TradeStatus_TradeStatusID foreign key references dbo.TradeStatus(TradeStatusID),
    CreateUserID int not null constraint FK_Trade_User_CreateUserID_UserID foreign key references dbo.[User](UserID),
    TradeNumber varchar(50) null
)
