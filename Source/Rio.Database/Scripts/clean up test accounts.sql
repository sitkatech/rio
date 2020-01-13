delete from dbo.accountuser where accountid in (select accountid from dbo.account where updatedate is null)

delete from dbo.account where updatedate is null