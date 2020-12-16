using System.Collections.Generic;

namespace Rio.API.GeoSpatial
{
    public class FeatureClassInfo
    {
        public string LayerName { get; set; }
        public string FeatureType { get; set; }
        public int FeatureCount { get; set; }
        public List<string> Columns { get; set; }
    }
}