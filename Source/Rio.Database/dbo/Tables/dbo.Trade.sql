CREATE TABLE [dbo].[Trade]
(
    TradeID INT NOT NULL identity(1,1) constraint PK_Trade_TradeID primary key,
    PostingID int not null constraint FK_Trade_Posting_PostingID foreign key references dbo.Posting(PostingID),
    TradeDate datetime not null,
    TradeStatusID int not null  constraint FK_Trade_TradeStatus_TradeStatusID foreign key references dbo.TradeStatus(TradeStatusID),
    CreateAccountID int not null constraint FK_Trade_Account_CreateAccountID_AccountID foreign key references dbo.[Account](AccountID),
    TradeNumber varchar(50) null
)
