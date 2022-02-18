create table dbo.Tag(
	TagID int not null identity(1,1) constraint PK_Tag_TagID primary key,
	TagName varchar(100) not null constraint AK_Tag_TagName unique,
	TagDescription varchar(500) null
)

create table dbo.ParcelTag(
	ParcelTagID int not null identity(1,1) constraint PK_ParcelTag_ParcelTagID primary key,
	ParcelID int not null constraint FK_ParcelTag_Parcel_ParcelID foreign key references dbo.Parcel(ParcelID),
	TagID int not null constraint FK_ParcelTag_Tag_TagID foreign key references dbo.Tag(TagID)
)