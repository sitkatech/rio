alter table dbo.Account
add CreateDate datetime null
go

update dbo.Account
set CreateDate = case when UpdateDate is not null then UpdateDate else GETDATE() end

alter table dbo.Account
alter column CreateDate datetime not null

alter table dbo.Account
add InactivateDate datetime null

alter table dbo.Account
alter column AccountVerificationKey varchar(6) null