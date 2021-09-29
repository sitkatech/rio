SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WaterTransferRegistrationStatus](
	[WaterTransferRegistrationStatusID] [int] NOT NULL,
	[WaterTransferRegistrationStatusName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[WaterTransferRegistrationStatusDisplayName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusID] PRIMARY KEY CLUSTERED 
(
	[WaterTransferRegistrationStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusDisplayName] UNIQUE NONCLUSTERED 
(
	[WaterTransferRegistrationStatusDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusName] UNIQUE NONCLUSTERED 
(
	[WaterTransferRegistrationStatusName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
