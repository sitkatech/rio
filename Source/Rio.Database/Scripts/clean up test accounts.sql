delete from accountuser where accountid in (select accountid from account where updatedate is null)

delete from account where updatedate is null