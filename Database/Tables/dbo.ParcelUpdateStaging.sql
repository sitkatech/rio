SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelUpdateStaging](
	[ParcelUpdateStagingID] [int] IDENTITY(1,1) NOT NULL,
	[ParcelNumber] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParcelGeometry] [geometry] NOT NULL,
	[OwnerName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
<<<<<<< HEAD
	[ParcelGeometry4326] [geometry] NULL,
=======
	[ParcelGeometry4326] [geometry] NOT NULL,
>>>>>>> Found out initial parcel layer was projected in 32611, and under that assumption can now fully complete operation. Add helpers to cast 32611 to 4326, add script that does the parcel layer update officially, add constraint to ensure that any account that is Inactivated has an Inactivate date and any account that isn't inactive doesn't have an inactivate date, make account verification key nullable but add constraint to enforce uniqueness, remove constraint from accountparcel table that requires either an accountid or ownername (may go back on this later)
	[ParcelGeometryText] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParcelGeometry4326Text] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ParcelUpdateStaging_ParcelUpdateStagingID] PRIMARY KEY CLUSTERED 
(
	[ParcelUpdateStagingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ParcelUpdateStaging_ParcelNumber] UNIQUE NONCLUSTERED 
(
	[ParcelNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
