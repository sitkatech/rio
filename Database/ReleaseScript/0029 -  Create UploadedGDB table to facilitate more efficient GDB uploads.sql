create table dbo.UploadedGdb
(
	UploadedGdbID int not null constraint PK_UploadedGdb_UploadedGdbID primary key,
	GdbFileContents varbinary(max) null,
	UploadDate Datetime not null
)
GO