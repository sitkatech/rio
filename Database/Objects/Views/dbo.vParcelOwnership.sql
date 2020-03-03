if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelOwnership'))
	drop view dbo.vParcelOwnership
go

Create View dbo.vParcelOwnership
as

SELECT 
IsNull([AccountParcelID], -999) as ID, -- for when Microsoft fixes EF Core Views so they can be involved in navigational properties.
    [AccountParcelID],
	[AccountID],
	[ParcelID],
	[OwnerName],
	[EffectiveYear],
	[SaleDate],
	[Note],
	Row_Number() OVER (
		Partition by ParcelID Order by isnull(SaleDate,0) desc
	) as RowNumber
  FROM [dbo].[AccountParcel]
GO
/*
select *
from dbo.vPostingDetailed
*/