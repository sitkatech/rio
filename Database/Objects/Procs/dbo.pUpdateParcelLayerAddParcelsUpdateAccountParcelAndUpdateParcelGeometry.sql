IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry'))
    drop procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
go

create procedure dbo.pUpdateParcelLayerAddParcelsUpdateAccountParcelAndUpdateParcelGeometry
<<<<<<< HEAD
(
    @year int
)
=======
>>>>>>> Found out initial parcel layer was projected in 32611, and under that assumption can now fully complete operation. Add helpers to cast 32611 to 4326, add script that does the parcel layer update officially, add constraint to ensure that any account that is Inactivated has an Inactivate date and any account that isn't inactive doesn't have an inactivate date, make account verification key nullable but add constraint to enforce uniqueness, remove constraint from accountparcel table that requires either an accountid or ownername (may go back on this later)
as

begin

<<<<<<< HEAD
	insert into dbo.Parcel (ParcelNumber, ParcelGeometry, ParcelAreaInSquareFeet, ParcelAreaInAcres)
	select pus.ParcelNumber, pus.ParcelGeometry4326, round(pus.ParcelGeometry.STArea() * 10.764, 0), round(pus.ParcelGeometry.STArea() / 4047, 14) 
=======
	insert into dbo.Parcel (ParcelNumber, ParcelGeometry, OwnerName, OwnerAddress, OwnerCity, OwnerZipCode, ParcelAreaInSquareFeet, ParcelAreaInAcres)
	select pus.ParcelNumber, pus.ParcelGeometry4326, pus.OwnerName, 'a', 'a', 'a', round(pus.ParcelGeometry.STArea() * 10.764, 0), round(pus.ParcelGeometry.STArea() / 4047, 14) 
>>>>>>> Found out initial parcel layer was projected in 32611, and under that assumption can now fully complete operation. Add helpers to cast 32611 to 4326, add script that does the parcel layer update officially, add constraint to ensure that any account that is Inactivated has an Inactivate date and any account that isn't inactive doesn't have an inactivate date, make account verification key nullable but add constraint to enforce uniqueness, remove constraint from accountparcel table that requires either an accountid or ownername (may go back on this later)
	from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
	join dbo.ParcelUpdateStaging pus on v.ParcelNumber = pus.ParcelNumber
	where OldOwnerName is null and OldGeometryText is null

<<<<<<< HEAD
	insert into dbo.AccountParcel (AccountID, ParcelID, EffectiveYear, SaleDate, ParcelStatusID)
	select a.AccountID, p.ParcelID, @year, GETDATE(), case when NewOwnerName is null then 2 else 1 end
	from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
	join dbo.Parcel p on v.ParcelNumber = p.ParcelNumber
	left join dbo.Account a on v.NewOwnerName = a.AccountName
	where (OldOwnerName is not null and NewOwnerName is null) or --deactivate case
	(OldOwnerName is null and NewOwnerName is not null) or --brand new case or existed without an owner
	OldOwnerName <> NewOwnerName
=======

	insert into dbo.AccountParcel (AccountID, ParcelID, EffectiveYear, SaleDate)
	select a.AccountID, p.ParcelID, 2020, GETDATE()
	from dbo.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry v
	join dbo.Parcel p on v.ParcelNumber = p.ParcelNumber
	left join dbo.Account a on v.NewOwnerName = a.AccountName
	where (OldOwnerName is null and NewOwnerName is not null) or OldOwnerName <> NewOwnerName
>>>>>>> Found out initial parcel layer was projected in 32611, and under that assumption can now fully complete operation. Add helpers to cast 32611 to 4326, add script that does the parcel layer update officially, add constraint to ensure that any account that is Inactivated has an Inactivate date and any account that isn't inactive doesn't have an inactivate date, make account verification key nullable but add constraint to enforce uniqueness, remove constraint from accountparcel table that requires either an accountid or ownername (may go back on this later)

	update dbo.Parcel
	set ParcelGeometry = pus.ParcelGeometry4326,
	ParcelAreaInSquareFeet = round(pus.ParcelGeometry.STArea() * 10.764, 0),
	ParcelAreaInAcres = round(pus.ParcelGeometry.STArea() / 4047, 14)
	from dbo.ParcelUpdateStaging pus
	join dbo.Parcel p on pus.ParcelNumber = p.ParcelNumber

end