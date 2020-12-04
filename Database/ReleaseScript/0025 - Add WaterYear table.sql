create table dbo.WaterYear (
	WaterYearID int not null identity(1,1) constraint PK_WaterYear_WaterYearID primary key,
	[Year] int not null constraint AK_WaterYear_Year unique,
	FinalizeDate Datetime null
)

insert into dbo.WaterYear([Year])
values (2017), (2018), (2019), (2020), (2021)