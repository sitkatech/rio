create table dbo.CimisPrecipitationDatum
(
	CimisPrecipitationDatumID int not null identity(1,1) constraint PK_CimisPrecipitationDatum_CimisPrecipitationDatumID primary key,
	DateMeasured datetime not null,
	Precipitation decimal (5, 2) not null,
	LastUpdated datetime not null
)
GO
