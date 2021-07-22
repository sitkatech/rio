using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Keyless]
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
