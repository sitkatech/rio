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
    public partial class vGeoServerWell
    {
        public int PrimaryKey { get; set; }
        public int WellID { get; set; }
        [Required]
        [StringLength(50)]
        public string WellName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry WellGeometry { get; set; }
    }
}
