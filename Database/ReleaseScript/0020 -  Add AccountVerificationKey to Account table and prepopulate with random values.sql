alter table dbo.Account
add AccountVerificationKey varchar(6) null
go

update dbo.Account
set AccountVerificationKey = concat(
	char(65 + cast(substring(cast(AccountNumber as varchar), 5, 1) as int)),
	char(65 + cast(substring(cast(AccountNumber as varchar), 4, 1) as int)),
	char(65 + cast(substring(cast(AccountNumber as varchar), 3, 1) as int)),
	RIGHT('000'+CAST((AccountID * AccountNumber) % 1000 AS VARCHAR(3)),3)
	)
go

alter table dbo.Account
alter column AccountVerificationKey varchar(6) not null
go

alter table dbo.Account
add constraint AK_Account_AccountVerificationKey unique(AccountVerificationKey)
go