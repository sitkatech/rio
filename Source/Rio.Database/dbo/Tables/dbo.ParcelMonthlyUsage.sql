CREATE TABLE [dbo].[ParcelMonthlyUsage]
(
    ParcelMonthlyUsageID INT NOT NULL identity(1, 1) constraint PK_ParcelMonthlyUsage_ParcelMonthlyUsageID primary key,
    ParcelID int not null constraint FK_ParcelMonthlyUsage_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
    WaterYear int not null,
    WaterMonth int not null,
    AcreFeetUsed decimal(10, 4) not null,
    constraint AK_ParcelMonthlyUsage_ParcelID_WaterYear_WaterMonth unique(ParcelID, WaterYear, WaterMonth)
)
GO