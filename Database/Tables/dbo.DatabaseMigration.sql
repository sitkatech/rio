SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseMigration](
	[DatabaseMigrationNumber] [int] IDENTITY(1,1) NOT NULL,
	[ReleaseScriptFileName] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DateMigrated] [datetime2](7) NOT NULL,
	[MigrationAuthorName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MigrationReason] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DatabaseMigration_DatabaseMigrationNumber] PRIMARY KEY CLUSTERED 
(
	[DatabaseMigrationNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_DatabaseMigration_ReleaseScriptFileName] UNIQUE NONCLUSTERED 
(
	[ReleaseScriptFileName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[DatabaseMigration] ADD  DEFAULT (getutcdate()) FOR [DateMigrated]