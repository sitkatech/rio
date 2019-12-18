CREATE TABLE [dbo].[AccountUser]
(
    AccountUserID INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_AccountUser_AccountUserID PRIMARY KEY,
    UserID INT NOT NULL CONSTRAINT FK_AccountUser_User_UserID FOREIGN KEY REFERENCES dbo.[User](UserID),
    AccountID INT NOT NULL CONSTRAINT FK_AccountUser_Account_AccountID FOREIGN KEY REFERENCES dbo.Account(AccountID)
)
