SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelAllocationType](
	[ParcelAllocationTypeID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelAllocationTypeName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsAppliedProportionally] [bit] NOT NULL,
	[ParcelAllocationTypeDefinition] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsSourcedFromApi] [bit] NOT NULL,
 CONSTRAINT [PK_ParcelAllocationType_ParcelAllocationTypeID] PRIMARY KEY CLUSTERED 
(
	[ParcelAllocationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ParcelAllocationType_ParcelAllocationTypeName] UNIQUE NONCLUSTERED 
(
	[ParcelAllocationTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX [CK_ParcelAllocationType_AtMostOne_IsSourcedFromApi_True] ON [dbo].[ParcelAllocationType]
(
	[IsSourcedFromApi] ASC
)
WHERE ([IsSourcedFromApi]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]