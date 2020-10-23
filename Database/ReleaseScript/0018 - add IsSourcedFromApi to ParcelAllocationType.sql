Alter Table dbo.ParcelAllocationType
Add IsSourcedFromApi bit null
go

Update dbo.ParcelAllocationType
Set IsSourcedFromApi = 0
go

Alter Table dbo.ParcelAllocationType
Alter Column IsSourcedFromApi bit not null
go

Create Unique Nonclustered Index CK_ParcelAllocationType_AtMostOne_IsSourcedFromApi_True
on dbo.ParcelAllocationType(IsSourcedFromApi) Where IsSourcedFromApi = 1

Alter Table dbo.ParcelAllocationType
Add SortOrder int null
go

Update dbo.ParcelAllocationType
Set SortOrder = ParcelAllocationTypeID
go

Alter Table dbo.ParcelAllocationType
Alter Column SortOrder int not null
go