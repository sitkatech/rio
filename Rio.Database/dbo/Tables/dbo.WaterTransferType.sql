CREATE TABLE [dbo].[WaterTransferType](
	[WaterTransferTypeID] [int] NOT NULL CONSTRAINT [PK_WaterTransferType_WaterTransferTypeID] PRIMARY KEY,
	[WaterTransferTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_WaterTransferType_WaterTransferTypeName] UNIQUE,
	[WaterTransferTypeDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_WaterTransferType_WaterTransferTypeDisplayName] UNIQUE
)
