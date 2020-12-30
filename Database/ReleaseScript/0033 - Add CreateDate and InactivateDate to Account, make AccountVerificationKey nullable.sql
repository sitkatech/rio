alter table dbo.Account
add CreateDate datetime null
go

update dbo.Account
set CreateDate = case when UpdateDate is not null then UpdateDate else GETDATE() end

alter table dbo.Account
alter column CreateDate datetime not null

alter table dbo.Account
add InactivateDate datetime null
go

ALTER TABLE [dbo].[Account]  WITH CHECK ADD CONSTRAINT [CK_InactivateDate_AccountStatusInactive] CHECK  (([AccountStatusID] = 2 and [InactivateDate] is not null) or ([AccountStatusID] <> 2 and [InactivateDate] is null))

alter table dbo.Account
alter column AccountVerificationKey varchar(6) null

alter table dbo.Account
drop constraint [AK_Account_AccountVerificationKey]

create unique nonclustered index [AK_Account_AccountVerificationKey] 
on dbo.Account(AccountVerificationKey)
<<<<<<< HEAD
where AccountVerificationKey is not null
=======
where AccountVerificationKey is not null
>>>>>>> Found out initial parcel layer was projected in 32611, and under that assumption can now fully complete operation. Add helpers to cast 32611 to 4326, add script that does the parcel layer update officially, add constraint to ensure that any account that is Inactivated has an Inactivate date and any account that isn't inactive doesn't have an inactivate date, make account verification key nullable but add constraint to enforce uniqueness, remove constraint from accountparcel table that requires either an accountid or ownername (may go back on this later)
