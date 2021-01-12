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

update dbo.Account
set InactivateDate = UpdateDate
from dbo.Account
where AccountStatusID = 2

ALTER TABLE [dbo].[Account]  WITH CHECK ADD CONSTRAINT [CK_InactivateDate_AccountStatusInactive] CHECK  (([AccountStatusID] = 2 and [InactivateDate] is not null) or ([AccountStatusID] <> 2 and [InactivateDate] is null))

alter table dbo.Account
alter column AccountVerificationKey varchar(6) null

alter table dbo.Account
drop constraint [AK_Account_AccountVerificationKey]

create unique nonclustered index [AK_Account_AccountVerificationKey] 
on dbo.Account(AccountVerificationKey)
where AccountVerificationKey is not null