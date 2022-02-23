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
    public partial class vGeoServerScenarioArsenicContaminationLocation
    {
        public int PrimaryKey { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationLocationWellName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ScenarioArsenicContaminationLocationGeometry { get; set; }
    }
}
