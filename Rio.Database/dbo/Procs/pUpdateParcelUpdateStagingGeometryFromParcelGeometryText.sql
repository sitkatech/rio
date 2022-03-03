create procedure dbo.pUpdateParcelUpdateStagingGeometryFromParcelGeometryText
as

begin
	update dbo.ParcelUpdateStaging
	set ParcelGeometry = geometry::STGeomFromText(ParcelGeometryText, 2229).MakeValid(),
	ParcelGeometry4326 = geometry::STGeomFromText(ParcelGeometry4326Text, 4326).MakeValid()
end 
