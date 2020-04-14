using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class vGeoServerDisadvantagedCommunity
    {
        public int PrimaryKey { get; set; }
        [Required]
        [StringLength(100)]
        public string DisadvantagedCommunityName { get; set; }
        public int LSADCode { get; set; }
        [Required]
        [StringLength(100)]
        public string DisadvantagedCommunityStatusName { get; set; }
        [Required]
        [StringLength(10)]
        public string GeoServerLayerColor { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry DisadvantagedCommunityGeometry { get; set; }
    }
}
