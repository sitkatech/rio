SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Parcel](
	[ParcelID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelNumber] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParcelGeometry] [geometry] NOT NULL,
	[OwnerName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OwnerAddress] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OwnerCity] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OwnerZipCode] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParcelAreaInSquareFeet] [int] NOT NULL,
	[ParcelAreaInAcres] [float] NOT NULL,
	[ParcelStatusID] [int] NOT NULL,
 CONSTRAINT [PK_Parcel_ParcelID] PRIMARY KEY CLUSTERED 
(
	[ParcelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Parcel_ParcelNumber] UNIQUE NONCLUSTERED 
(
	[ParcelNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Parcel]  WITH CHECK ADD  CONSTRAINT [FK_Parcel_ParcelStatus_ParcelStatusID] FOREIGN KEY([ParcelStatusID])
REFERENCES [dbo].[ParcelStatus] ([ParcelStatusID])
GO
ALTER TABLE [dbo].[Parcel] CHECK CONSTRAINT [FK_Parcel_ParcelStatus_ParcelStatusID]