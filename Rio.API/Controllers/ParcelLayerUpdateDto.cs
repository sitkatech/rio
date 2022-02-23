using System.Collections.Generic;

namespace Rio.API.Controllers
{
    public class ParcelLayerUpdateDto
    {
        public string ParcelLayerNameInGDB { get; set; }
        public int UploadedGDBID { get; set; }
        public List<ParcelRequiredColumnAndMappingDto> ColumnMappings { get; set; }
        public int YearChangesToTakeEffect { get; set; }
    }
}