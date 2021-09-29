SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelAllocation](
	[ParcelAllocationID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelID] [int] NOT NULL,
	[WaterYear] [int] NOT NULL,
	[ParcelAllocationTypeID] [int] NOT NULL,
	[AcreFeetAllocated] [decimal](10, 4) NOT NULL,
 CONSTRAINT [PK_ParcelAllocation_ParcelAllocationID] PRIMARY KEY CLUSTERED 
(
	[ParcelAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ParcelAllocation_ParcelID_WaterYear] UNIQUE NONCLUSTERED 
(
	[ParcelID] ASC,
	[WaterYear] ASC,
	[ParcelAllocationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ParcelAllocation]  WITH CHECK ADD  CONSTRAINT [FK_ParcelAllocation_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[ParcelAllocation] CHECK CONSTRAINT [FK_ParcelAllocation_Parcel_ParcelID]
GO
ALTER TABLE [dbo].[ParcelAllocation]  WITH CHECK ADD  CONSTRAINT [FK_ParcelAllocation_ParcelAllocationType_ParcelAllocationTypeID] FOREIGN KEY([ParcelAllocationTypeID])
REFERENCES [dbo].[ParcelAllocationType] ([ParcelAllocationTypeID])
GO
ALTER TABLE [dbo].[ParcelAllocation] CHECK CONSTRAINT [FK_ParcelAllocation_ParcelAllocationType_ParcelAllocationTypeID]