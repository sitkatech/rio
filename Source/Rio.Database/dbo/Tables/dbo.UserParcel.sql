CREATE TABLE [dbo].[UserParcel]
(
    UserParcelID INT NOT NULL identity(1, 1) constraint PK_UserParcel_UserParcelID primary key,
    UserID int not null constraint FK_UserParcel_User_UserID foreign key references dbo.[User](UserID),
    ParcelID int not null constraint FK_UserParcel_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
    constraint AK_UserParcel_UserID_ParcelID unique(UserID, ParcelID)
)
GO