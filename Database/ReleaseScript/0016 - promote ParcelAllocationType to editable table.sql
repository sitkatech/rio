-- Create temp table and insert rows from ParcelAllocationType
Create table dbo.TempPAT(
PATID int not null,
PATDisplayName varchar(max) not null,
PATName varchar(max) not null
)

Insert into dbo.TempPAT (PATID, PATDisplayName, PATName)
select * from dbo.ParcelAllocationType

-- drop foreign keys referencing ParcelAllocationType
ALTER TABLE [dbo].[ParcelAllocationHistory] DROP CONSTRAINT [FK_ParcelAllocationHistory_ParcelAllocationType_ParcelAllocationTypeID]
GO

ALTER TABLE [dbo].[ParcelAllocation] DROP CONSTRAINT [FK_ParcelAllocation_ParcelAllocationType_ParcelAllocationTypeID]
GO

-- drop ParcelAllocationType and recreate with an identity column. Get rid of "Display Name" relic while we're at it, too
drop table dbo.ParcelAllocationType

CREATE TABLE [dbo].[ParcelAllocationType](
	[ParcelAllocationTypeID] [int] NOT NULL identity(1,1),
	[ParcelAllocationTypeName] [varchar](50) NOT NULL
 CONSTRAINT [PK_ParcelAllocationType_ParcelAllocationTypeID] PRIMARY KEY CLUSTERED 
(
	[ParcelAllocationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ParcelAllocationType_ParcelAllocationTypeName] UNIQUE NONCLUSTERED 
(
	[ParcelAllocationTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- reinsert rows and drop temp table
SET IDENTITY_INSERT dbo.ParcelAllocationType ON

Insert into dbo.ParcelAllocationType (ParcelAllocationTypeID, ParcelAllocationTypeName)
select
PATID, PATName
from dbo.TempPAT

drop table dbo.TempPAT

SET IDENTITY_INSERT dbo.ParcelAllocationType OFF

-- reinstate foreign keys

ALTER TABLE [dbo].[ParcelAllocationHistory]  WITH CHECK ADD  CONSTRAINT [FK_ParcelAllocationHistory_ParcelAllocationType_ParcelAllocationTypeID] FOREIGN KEY([ParcelAllocationTypeID])
REFERENCES [dbo].[ParcelAllocationType] ([ParcelAllocationTypeID])
GO

ALTER TABLE [dbo].[ParcelAllocation]  WITH CHECK ADD  CONSTRAINT [FK_ParcelAllocation_ParcelAllocationType_ParcelAllocationTypeID] FOREIGN KEY([ParcelAllocationTypeID])
REFERENCES [dbo].[ParcelAllocationType] ([ParcelAllocationTypeID])
GO


-- add column for allocation method
Alter Table dbo.ParcelAllocationType
Add IsAppliedProportionally bit null
go

Update dbo.ParcelAllocationType
set IsAppliedProportionally = 1
where ParcelAllocationTypeName in ('Project Water', 'Native Yield')

Update dbo.ParcelAllocationType
set IsAppliedProportionally = 0
where ParcelAllocationTypeName in ('Reconciliation', 'Stored Water')

Alter table dbo.ParcelAllocationType
Alter Column IsAppliedProportionally bit not null

Alter table dbo.ParcelAllocationType
Add ParcelAllocationTypeDefinition varchar(max) null

Insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values
(8, 'ConfigureWaterTypes', 'Configure Water Types'),
(9, 'SetWaterAllocation', 'SetWaterAllocation')

Insert into dbo.CustomRichText(CustomRichTextTypeID)
values (8), (9)