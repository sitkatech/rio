IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pUpdateParcelUpdateStagingGeometryFromParcelGeometryText'))
    drop procedure dbo.pUpdateParcelUpdateStagingGeometryFromParcelGeometryText
go

create procedure dbo.pUpdateParcelUpdateStagingGeometryFromParcelGeometryText
as

begin
	update dbo.ParcelUpdateStaging
	set ParcelGeometry = geometry::STGeomFromText(ParcelGeometryText, 32611).MakeValid(),
	ParcelGeometry4326 = geometry::STGeomFromText(ParcelGeometry4326Text, 4326).MakeValid()
end 
