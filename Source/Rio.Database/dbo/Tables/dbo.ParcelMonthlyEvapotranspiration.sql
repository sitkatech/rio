CREATE TABLE [dbo].[ParcelMonthlyEvapotranspiration]
(
    ParcelMonthlyEvapotranspirationID INT NOT NULL identity(1, 1) constraint PK_ParcelMonthlyEvapotranspiration_ParcelMonthlyEvapotranspirationID primary key,
    ParcelID int not null constraint FK_ParcelMonthlyEvapotranspiration_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
    WaterYear int not null,
    WaterMonth int not null,
    EvapotranspirationRate decimal(10, 4) not null,
    constraint AK_ParcelMonthlyEvapotranspiration_ParcelID_WaterYear_WaterMonth unique(ParcelID, WaterYear, WaterMonth)
)
GO