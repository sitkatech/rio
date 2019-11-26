Drop View If Exists dbo.vParcelOwnership
GO --!

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
  FROM [RioDB].[dbo].[UserParcel]
