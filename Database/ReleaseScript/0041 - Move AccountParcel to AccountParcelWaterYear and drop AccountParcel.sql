create table dbo.AccountParcelWaterYear (
	AccountParcelWaterYearID int not null identity(1,1) constraint PK_AccountParcelWaterYear_AccountParcelWaterYear primary key,
	AccountID int not null constraint FK_AccountParcelWaterYear_Account_AccountID foreign key references dbo.Account (AccountID),
	ParcelID int not null constraint FK_AccountParcelWaterYear_Parcel_ParcelID foreign key references dbo.Parcel (ParcelID),
	WaterYearID int not null constraint FK_AccountParcelWaterYear_WaterYear_WaterYearID foreign key references dbo.WaterYear (WaterYearID),
	constraint AK_AccountParcelWaterYear_ParcelID_WaterYearID unique (ParcelID, WaterYearID) 
)

--At time of writing release script, Change Parcel Owner functionality is broken
--and no Accounts claim ownership of a parcel for an actual effectiveyear, so 
--we can just take everyone in the AccountParcel table and cross join with all
--the water years we know to exist
insert into dbo.AccountParcelWaterYear (AccountID, ParcelID, WaterYearID)
select ap.AccountID, ap.ParcelID, wy.WaterYearID
from dbo.WaterYear wy
cross join dbo.AccountParcel ap

drop table dbo.AccountParcel
