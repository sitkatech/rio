CREATE TABLE [dbo].[AccountStatus]
(
    AccountStatusID INT NOT NULL CONSTRAINT PK_AccountStatus_AccountStatusID PRIMARY KEY,
    AccountStatusName VARCHAR(20) NOT NULL CONSTRAINT AK_AccountStatus_AccountStatusName UNIQUE,
    AccountStatusDisplayName VARCHAR(20) NOT NULL CONSTRAINT AK_AccountStatus_AccountStatusDisplayName UNIQUE
)
