using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Rio.API.GeoSpatial
{
    public interface IHasGeometry
    {
        [JsonIgnore]
        Geometry Geometry { get; set; }
    }
}