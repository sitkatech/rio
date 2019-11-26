if exists (select * from dbo.sysobjects where id = object_id('dbo.vParcelOwnership'))
	drop view dbo.vParcelOwnership
go

Create View dbo.vParcelOwnership
as

SELECT [UserParcelID],
	[UserID],
	[ParcelID],
	[OwnerName],
	[EffectiveYear],
	[SaleDate],
	[Note],
	Row_Number() OVER (
		Partition by ParcelID Order by isnull(SaleDate,0) desc
	) as RowNumber
  FROM [dbo].[UserParcel]
GO
/*
select *
from dbo.vPostingDetailed
*/