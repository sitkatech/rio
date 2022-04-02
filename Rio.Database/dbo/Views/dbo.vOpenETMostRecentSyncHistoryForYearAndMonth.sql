create view dbo.vOpenETMostRecentSyncHistoryForYearAndMonth
as

select openetsh.OpenETSyncHistoryID, openetsh.WaterYearMonthID, openetsh.OpenETSyncResultTypeID, openetsh.CreateDate, openetsh.UpdateDate, openetsh.GoogleBucketFileRetrievalURL, openetsh.ErrorMessage
from dbo.OpenETSyncHistory openetsh
join 
(
	select WaterYearMonthID, max(CreateDate) CreateDate
	from dbo.OpenETSyncHistory
	group by WaterYearMonthID
) mostRecent on openetsh.WaterYearMonthID = mostRecent.WaterYearMonthID and openetsh.CreateDate = mostRecent.CreateDate

go