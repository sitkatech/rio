create table dbo.AccountTemp(
	RowNum int null,
	AccountID int null,
	AccountVerificationKey varchar(6) null
)

insert into dbo.AccountTemp (RowNum, AccountID)
select ROW_NUMBER() OVER(ORDER BY AccountID ASC) AS RowNum, AccountID
from dbo.Account
go

DECLARE @CurrentRowNum INT = 1;
DECLARE @RowCount BIGINT;
DECLARE @CountContainingKey INT = 1;
DECLARE @NewKey varchar(6);

DECLARE @AlphaUpperBound INT = 27;
DECLARE @AlphaLowerBound INT = 1;
DECLARE @IntUpperBound INT = 10;
DECLARE @IntLowerBound INT =  0;

SELECT @RowCount = COUNT(*) FROM dbo.AccountTemp;
 
WHILE @CurrentRowNum <= @RowCount
BEGIN
   WHILE @CountContainingKey > 0
   BEGIN
		SET @NewKey = (char(64 + ROUND(((@AlphaUpperBound-@AlphaLowerBound-1) * RAND() + @AlphaLowerBound), 0)) 
					+ char(64 + ROUND(((@AlphaUpperBound-@AlphaLowerBound-1) * RAND() + @AlphaLowerBound), 0))
					+ char(64 + ROUND(((@AlphaUpperBound-@AlphaLowerBound-1) * RAND() + @AlphaLowerBound), 0))
					+ CAST(ROUND(((@IntUpperBound-@IntLowerBound-1) * RAND()), + @IntLowerBound) as varchar)
					+ CAST(ROUND(((@IntUpperBound-@IntLowerBound-1) * RAND()), + @IntLowerBound) as varchar)
					+ CAST(ROUND(((@IntUpperBound-@IntLowerBound-1) * RAND()), + @IntLowerBound) as varchar))
		SELECT @CountContainingKey = COUNT(*) FROM dbo.AccountTemp WHERE AccountVerificationKey = @NewKey
   END
   UPDATE dbo.AccountTemp 
   SET AccountVerificationKey = @NewKey
   WHERE RowNum = @CurrentRowNum;
    
   SET @CurrentRowNum = @CurrentRowNum + 1 
   SET @CountContainingKey = 1
END

alter table dbo.Account
add AccountVerificationKey varchar(6) null
go

update dbo.Account
set AccountVerificationKey = [at].AccountVerificationKey
from dbo.AccountTemp [at]
join dbo.Account a on [at].AccountID = a.AccountID
go

alter table dbo.Account
alter column AccountVerificationKey varchar(6) not null
go

alter table dbo.Account
add constraint AK_Account_AccountVerificationKey unique(AccountVerificationKey)
go

drop table dbo.AccountTemp
go