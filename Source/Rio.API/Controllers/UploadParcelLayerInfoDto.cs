using System.Collections.Generic;
using Rio.API.GeoSpatial;

namespace Rio.API.Controllers
{
    public class UploadParcelLayerInfoDto
    {
        public int UploadedGdbID { get; set; }
        public List<FeatureClassInfo> FeatureClasses { get; set; }
    }
}