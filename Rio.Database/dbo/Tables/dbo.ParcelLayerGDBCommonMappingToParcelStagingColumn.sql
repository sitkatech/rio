CREATE TABLE [dbo].[ParcelLayerGDBCommonMappingToParcelStagingColumn](
	[ParcelLayerGDBCommonMappingToParcelColumnID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelLayerGDBCommonMappingToParcelColumnID] PRIMARY KEY,
	[ParcelNumber] [varchar](100) NOT NULL CONSTRAINT [AK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelNumber] UNIQUE,
	[OwnerName] [varchar](100) NOT NULL CONSTRAINT [AK_ParcelLayerGDBCommonMappingToParcelColumn_OwnerName] UNIQUE
)
