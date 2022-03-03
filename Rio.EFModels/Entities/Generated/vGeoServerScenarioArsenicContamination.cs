using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class vGeoServerScenarioArsenicContamination
    {
        public int PrimaryKey { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationWellID { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationWellTypeName { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationSourceName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ScenarioArsenicContaminationGeometry { get; set; }
        public double? ScenarioArsenicContaminationContaminationConcentration { get; set; }
    }
}
