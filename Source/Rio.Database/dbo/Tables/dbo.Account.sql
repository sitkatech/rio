CREATE TABLE [dbo].[Account]
(
    AccountID INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_Account_AccountID PRIMARY KEY,
    -- SQL doesn't allow multiple IDENTITY columns, so letting AccountNumber be computed provides the same result
    AccountNumber as AccountId + 10000 CONSTRAINT AK_Account_AccountNumber UNIQUE,
    AccountName VARCHAR(255) NULL,
    AccountStatusID INT NOT NULL CONSTRAINT FK_Account_AccountStatus_AccountStatusID FOREIGN KEY REFERENCES dbo.AccountStatus(AccountStatusID),
    Notes VARCHAR(MAX) NULL
)
