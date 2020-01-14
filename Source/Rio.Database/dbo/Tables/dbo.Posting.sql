CREATE TABLE [dbo].[Posting]
(
    PostingID INT NOT NULL identity(1,1) constraint PK_Posting_PostingID primary key,
    PostingTypeID int not null constraint FK_Posting_PostingType_PostingTypeID foreign key references dbo.PostingType(PostingTypeID),
    PostingDate datetime not null,
    CreateAccountID int not null constraint FK_Posting_Account_CreateAccountID_AccountID foreign key references dbo.[Account](AccountID),
    Quantity int not null,
    Price money not null,
    PostingDescription varchar(2000) null,
    PostingStatusID int not null constraint FK_Posting_PostingStatus_PostingStatusID foreign key references dbo.PostingStatus(PostingStatusID), 
    AvailableQuantity int not null
)
