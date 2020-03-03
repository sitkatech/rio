print 'Getting logins to create'
if object_id('tempdb.dbo.#accountsToCreate') is not null drop table #accountsToCreate
select '${db-user}' as AccountName, 'S' as AccountType
into #accountsToCreate
union select '${db-batch-user}', 'U'
union select 'Sitka\H2O QA Support', 'G'
union select 'Sitka\H2O QA Tester', 'G'

-- Clear out any existing logins
print 'Clearing out any existing logins'

if object_id('tempdb.dbo.#allAccounts') is not null drop table #allAccounts
select a.AccountName, a.AccountType
into #allAccounts
from
(
    select 'RiowWebLocal' as AccountName, 'S' as AccountType
    union select 'Sitka\RioBatchQA', 'U'
    union select 'Sitka\RioBatchProd', 'U'
    union select 'RioTempTest', 'S'
    union select 'Sitka\H2O QA Support', 'G'
    union select 'Sitka\H2O QA Tester', 'G'
    union select 'Sitka\H2O Production Support', 'G'
    union select 'Sitka\TRPA Tech Leads', 'G'
) a left join #accountsToCreate ac on a.AccountName = ac.AccountName
where ac.AccountName is null


declare @accountName varchar(200)
declare @accountType varchar(2)
declare @sql nvarchar(1000)

while exists(select 1 from #allAccounts)
begin
    select top 1 @accountName = AccountName, @accountType = AccountType from #allAccounts

    print 'Login: ' + @accountName
    
    if exists (select 1 from sys.server_principals where name = @accountName and type = @accountType)
    begin
        set @sql = replace('drop login [ACCOUNT]', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql
    end
    if exists (select 1 from sys.sysusers where name = @accountName)
    begin
        set @sql = replace('drop user [ACCOUNT]', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql
    end
    delete from #allAccounts where AccountName = @accountName
end



-- now add the user for this environment specified by ${db-user} token
print 'Creating logins for this environment'


while exists(select 1 from #accountsToCreate)
begin
    select top 1 @accountName = AccountName, @accountType = AccountType from #accountsToCreate

    print 'Login: ' + @accountName

    if not exists (select 1 from sys.server_principals where name = @accountName and type = @accountType)
    begin
        set @sql = replace('create login [ACCOUNT] from windows', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql
    end

    if not exists (select 1 from sys.sysusers where name = @accountName)
    begin
        set @sql = replace('create user [ACCOUNT] for login [ACCOUNT]', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql    
    end
    
    exec sp_addrolemember 'db_owner', @accountName
    print 'added user [' + @accountName + '] to db_owner role'

    delete from #accountsToCreate where AccountName = @accountName
end