exec sp_rename 'dbo.PK_ParcelAllocationType_ParcelAllocationTypeID', 'PK_WaterType_WaterTypeID', 'OBJECT';
exec sp_rename 'dbo.AK_ParcelAllocationType_ParcelAllocationTypeName', 'AK_WaterType_WaterTypeName', 'OBJECT';
exec sp_rename 'dbo.FK_ParcelAllocationHistory_ParcelAllocationType_ParcelAllocationTypeID', 'FK_ParcelAllocationHistory_WaterType_WaterTypeID', 'OBJECT';
exec sp_rename 'dbo.FK_ParcelAllocation_ParcelAllocationType_ParcelAllocationTypeID', 'FK_ParcelAllocation_WaterType_WaterTypeID', 'OBJECT';
exec sp_rename 'dbo.ParcelAllocation.ParcelAllocationTypeID', 'WaterTypeID', 'COLUMN';
exec sp_rename 'dbo.ParcelAllocationType.ParcelAllocationTypeID', 'WaterTypeID', 'COLUMN';
exec sp_rename 'dbo.ParcelAllocationType.ParcelAllocationTypeName', 'WaterTypeName', 'COLUMN';
exec sp_rename 'dbo.ParcelAllocationType.ParcelAllocationTypeDefinition', 'WaterTypeDefinition', 'COLUMN';
exec sp_rename 'dbo.ParcelAllocationHistory.ParcelAllocationTypeID', 'WaterTypeID', 'COLUMN';
exec sp_rename 'dbo.ParcelAllocationType', 'WaterType';
GO

DROP INDEX CK_ParcelAllocationType_AtMostOne_IsSourcedFromApi_True ON dbo.WaterType
GO

CREATE UNIQUE NONCLUSTERED INDEX CK_WaterType_AtMostOne_IsSourcedFromApi_True ON dbo.WaterType
(
	IsSourcedFromApi ASC
)
WHERE (IsSourcedFromApi=(1))
GO

