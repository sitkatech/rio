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
        [Column(TypeName = "geometry")]
        public Geometry ScenarioArsenicContaminationGeometry { get; set; }
        [StringLength(50)]
        public string ScenarioArsenicContaminationWellID { get; set; }
        public double? ScenarioArsenicContaminationContaminationConcentration { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ScenarioArsenicContaminationDateAssessed { get; set; }
        [StringLength(50)]
        public string ScenarioArsenicContaminationUnits { get; set; }
        public double ScenarioArsenicContaminationLatitude { get; set; }
        public double ScenarioArsenicContaminationLongitude { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationWellTypeName { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationSourceName { get; set; }
    }
}
