SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelMonthlyEvapotranspiration](
	[ParcelMonthlyEvapotranspirationID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelID] [int] NOT NULL,
	[WaterYear] [int] NOT NULL,
	[WaterMonth] [int] NOT NULL,
	[EvapotranspirationRate] [decimal](10, 4) NULL,
	[OverriddenEvapotranspirationRate] [decimal](10, 4) NULL,
 CONSTRAINT [PK_ParcelMonthlyEvapotranspiration_ParcelMonthlyEvapotranspirationID] PRIMARY KEY CLUSTERED 
(
	[ParcelMonthlyEvapotranspirationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ParcelMonthlyEvapotranspiration_ParcelID_WaterYear_WaterMonth] UNIQUE NONCLUSTERED 
(
	[ParcelID] ASC,
	[WaterYear] ASC,
	[WaterMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ParcelMonthlyEvapotranspiration]  WITH CHECK ADD  CONSTRAINT [FK_ParcelMonthlyEvapotranspiration_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO
ALTER TABLE [dbo].[ParcelMonthlyEvapotranspiration] CHECK CONSTRAINT [FK_ParcelMonthlyEvapotranspiration_Parcel_ParcelID]