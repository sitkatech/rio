CREATE TABLE [dbo].[ParcelMonthlyEvapotranspirationOverride](
	[ParcelMonthlyEvapotranspirationOverrideID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelID] [int] NOT NULL,
	[WaterYear] [int] NOT NULL,
	[WaterMonth] [int] NOT NULL,
	[OverriddenEvapotranspirationRate] [decimal](10, 4) NOT NULL,
 CONSTRAINT [PK_ParcelMonthlyEvapotranspirationOverride_ParcelMonthlyEvapotranspirationOverrideID] PRIMARY KEY CLUSTERED 
(
	[ParcelMonthlyEvapotranspirationOverrideID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ParcelMonthlyEvapotranspirationOverride_ParcelID_WaterYear_WaterMonth] UNIQUE NONCLUSTERED 
(
	[ParcelID] ASC,
	[WaterYear] ASC,
	[WaterMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ParcelMonthlyEvapotranspirationOverride]  WITH CHECK ADD  CONSTRAINT [FK_ParcelMonthlyEvapotranspirationOverride_Parcel_ParcelID] FOREIGN KEY([ParcelID])
REFERENCES [dbo].[Parcel] ([ParcelID])
GO

ALTER TABLE [dbo].[ParcelMonthlyEvapotranspirationOverride] CHECK CONSTRAINT [FK_ParcelMonthlyEvapotranspirationOverride_Parcel_ParcelID]
GO
