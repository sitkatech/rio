CREATE TABLE dbo.UserParcel
(
    UserParcelID INT NOT NULL identity(1, 1) constraint PK_UserParcel_UserParcelID primary key,
    UserID int null constraint FK_UserParcel_User_UserID foreign key references dbo.[User](UserID),
    ParcelID int not null constraint FK_UserParcel_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
    OwnerName varchar(214) NULL,
	EffectiveYear int NULL,
    SaleDate datetime NULL,
	Note varchar(500) NULL,
    constraint CK_ParcelOwner_OwnerNameXorOwnerID check (
        (UserID IS NULL AND OwnerName IS NOT NULL) OR
        (UserID IS NOT NULL AND OwnerName IS NULL)
    )
    --constraint AK_UserParcel_UserID_ParcelID unique(UserID, ParcelID)
)
GO