using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class ScenarioArsenicContaminationLocation
    {
        [Key]
        public int ScenarioArsenicContaminationLocationID { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationLocationWellName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ScenarioArsenicContaminationLocationGeometry { get; set; }
    }
}
