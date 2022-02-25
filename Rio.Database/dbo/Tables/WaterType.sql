CREATE TABLE [dbo].[WaterType](
	[WaterTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterType_WaterTypeID] PRIMARY KEY,
	[WaterTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_WaterType_WaterTypeName] UNIQUE,
	[IsAppliedProportionally] [bit] NOT NULL,
	[WaterTypeDefinition] [varchar](max) NULL,
	[IsSourcedFromApi] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
)