CREATE TABLE [dbo].[Posting]
(
    PostingID INT NOT NULL identity(1,1) constraint PK_Posting_PostingID primary key,
    PostingTypeID int not null constraint FK_Posting_PostingType_PostingTypeID foreign key references dbo.PostingType(PostingTypeID),
    PostingDate datetime not null,
    CreateUserID int not null constraint FK_Posting_User_CreateUserID_UserID foreign key references dbo.[User](UserID),
    Quantity int not null,
    Price money not null,
    PostingDescription varchar(2000) not null,
    PostingStatusID int not null constraint FK_Posting_PostingStatus_PostingStatusID foreign key references dbo.PostingStatus(PostingStatusID)
)
