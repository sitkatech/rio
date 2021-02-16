delete from dbo.ParcelUpdateStaging

alter table dbo.ParcelUpdateStaging
add HasConflict bit not null

alter table dbo.ParcelUpdateStaging
drop constraint AK_ParcelUpdateStaging_ParcelNumber

create table dbo.AccountReconciliation (
	AccountReconciliationID int not null identity (1,1) constraint PK_AccountReconciliation_AccountReconciliationID primary key,
	ParcelID int not null constraint FK_AccountReconciliation_Parcel_ParcelID foreign key references dbo.Parcel (ParcelID),
	AccountID int not null constraint FK_AccountReconciliation_Account_AccountID foreign key references dbo.Account (AccountID)
)