CREATE TABLE [dbo].[ParcelAllocation]
(
    ParcelAllocationID INT NOT NULL identity(1, 1) constraint PK_ParcelAllocation_ParcelAllocationID primary key,
    ParcelID int not null constraint FK_ParcelAllocation_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
    WaterYear int not null,
    AcreFeetAllocated decimal(10, 4) null,
    constraint AK_ParcelAllocation_ParcelID_WaterYear unique(ParcelID, WaterYear)
)
GO