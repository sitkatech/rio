-- replace UserParcel with AccountParcel

CREATE TABLE dbo.AccountParcel(
	AccountParcelID int IDENTITY(1,1) NOT NULL,
	AccountID int NULL,
	ParcelID int NOT NULL,
	OwnerName varchar(214) NULL,
	EffectiveYear int NULL,
	SaleDate datetime NULL,
	Note varchar(500) NULL,
 CONSTRAINT PK_AccountParcel_AccountParcelID PRIMARY KEY CLUSTERED 
(
	AccountParcelID ASC
)
)
GO

ALTER TABLE dbo.AccountParcel  WITH CHECK ADD  CONSTRAINT FK_AccountParcel_Parcel_ParcelID FOREIGN KEY(ParcelID)
REFERENCES dbo.Parcel (ParcelID)
GO

ALTER TABLE dbo.AccountParcel  WITH CHECK ADD  CONSTRAINT FK_AccountParcel_Account_AccountID FOREIGN KEY(AccountID)
REFERENCES dbo.Account (AccountID)
GO

ALTER TABLE dbo.AccountParcel  WITH CHECK ADD  CONSTRAINT CK_AccountParcel_OwnerNameXorOwnerID CHECK  ((AccountID IS NULL AND OwnerName IS NOT NULL OR AccountID IS NOT NULL AND OwnerName IS NULL))
GO

Insert Into dbo.AccountParcel (AccountID, ParcelID, OwnerName, EffectiveYear, SaleDate, Note)
Select 
	AccountID,
	ParcelID,
	OwnerName,
	EffectiveYear,
	SaleDate,
	Note
from dbo.UserParcel up join dbo.AccountUser au
	on up.UserID = au.UserID

Drop Table dbo.UserParcel

 -- blast old testing data

delete from dbo.WaterTransferRegistrationParcel
delete from dbo.WaterTransferRegistration
delete from dbo.WaterTransfer
delete from dbo.Offer
delete from dbo.Trade
delete from dbo.Posting

-- Replace Posting.CreateUserID with CreateAccountID

Alter Table dbo.Posting
Add CreateAccountID int null
constraint FK_Posting_Account_CreateAccountID_AccountID foreign key references dbo.Account(AccountID)
GO

Update p
set p.CreateAccountID = au.AccountID
from dbo.Posting p join dbo.AccountUser au on p.CreateUserID = au.UserID
GO

Alter Table dbo.Posting
Alter column CreateAccountID int not null
Alter Table dbo.Posting
Drop Constraint FK_Posting_User_CreateUserID_UserID
Alter Table dbo.Posting
Drop Column CreateUserID

-- Replace Trade.CreateUserID with CreateAccountID

Alter Table dbo.Trade
Add CreateAccountID int null
constraint FK_Trade_Account_CreateAccountID_AccountID foreign key references dbo.Account(AccountID)
GO

Update p
set p.CreateAccountID = au.AccountID
from dbo.Trade p join dbo.AccountUser au on p.CreateUserID = au.UserID
GO

Alter Table dbo.Trade
Alter column CreateAccountID int not null
Alter Table dbo.Trade
Drop Constraint FK_Trade_User_CreateUserID_UserID
Alter Table dbo.Trade
Drop Column CreateUserID

-- Replace Offer.CreateUserID with CreateAccountID

Alter Table dbo.Offer
Add CreateAccountID int null
constraint FK_Offer_Account_CreateAccountID_AccountID foreign key references dbo.Account(AccountID)
GO

Update p
set p.CreateAccountID = au.AccountID
from dbo.Offer p join dbo.AccountUser au on p.CreateUserID = au.UserID
GO

Alter Table dbo.Offer
Alter column CreateAccountID int not null
Alter Table dbo.Offer
Drop Constraint FK_Offer_User_CreateUserID_UserID
Alter Table dbo.Offer
Drop Column CreateUserID



-- Replace WaterTransferRegistration.UserID with AccountID

Alter Table dbo.WaterTransferRegistration
Add AccountID int null
constraint FK_WaterTransferRegistration_Account_AccountID_AccountID foreign key references dbo.Account(AccountID)
GO

Update p
set p.AccountID = au.AccountID
from dbo.WaterTransferRegistration p join dbo.AccountUser au on p.UserID = au.UserID
GO

Alter Table dbo.WaterTransferRegistration
Alter column AccountID int not null
Alter Table dbo.WaterTransferRegistration
Drop Constraint FK_WaterTransferRegistration_User_UserID
Alter Table dbo.WaterTransferRegistration
Drop Column UserID