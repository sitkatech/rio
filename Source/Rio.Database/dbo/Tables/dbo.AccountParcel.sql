CREATE TABLE dbo.AccountParcel
(
    AccountParcelID INT NOT NULL identity(1, 1) constraint PK_AccountParcel_AccountParcelID primary key,
    AccountID int null constraint FK_AccountParcel_Account_AccountID foreign key references dbo.[Account](AccountID),
    ParcelID int not null constraint FK_AccountParcel_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
    OwnerName varchar(214) NULL,
	EffectiveYear int NULL,
    SaleDate datetime NULL,
	Note varchar(500) NULL,
    constraint CK_ParcelOwner_OwnerNameXorOwnerID check (
        (AccountID IS NULL AND OwnerName IS NOT NULL) OR
        (AccountID IS NOT NULL AND OwnerName IS NULL)
    )
)
