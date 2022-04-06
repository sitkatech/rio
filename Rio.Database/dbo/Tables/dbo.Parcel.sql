CREATE TABLE [dbo].[Parcel](
	[ParcelID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Parcel_ParcelID] PRIMARY KEY,
	[ParcelNumber] [varchar](20) NOT NULL CONSTRAINT [AK_Parcel_ParcelNumber] UNIQUE,
	[ParcelGeometry] [geometry] NOT NULL,
	[ParcelAreaInSquareFeet] [int] NOT NULL,
	[ParcelAreaInAcres] [float] NOT NULL,
	[ParcelStatusID] [int] NOT NULL CONSTRAINT [FK_Parcel_ParcelStatus_ParcelStatusID] FOREIGN KEY REFERENCES [dbo].[ParcelStatus] ([ParcelStatusID]),
	[InactivateDate] [datetime] NULL,
	CONSTRAINT [CK_ParcelStatus_ActiveXorInactiveAndInactivateDate] CHECK  (([ParcelStatusID]=(1) OR [ParcelStatusID]=(2) AND [InactivateDate] IS NOT NULL))
)
GO

CREATE SPATIAL INDEX SPATIAL_Parcel_ParcelGeometry ON dbo.Parcel (ParcelGeometry)
USING  GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-125, 31, -113, 45), CELLS_PER_OBJECT = 8)
