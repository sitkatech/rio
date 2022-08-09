using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    [Table("ScenarioArsenicContaminationLocation")]
    [Index("ScenarioArsenicContaminationLocationWellName", Name = "AK_ScenarioArsenicContaminationLocation_ScenarioArsenicContaminationLocationWellName", IsUnique = true)]
    public partial class ScenarioArsenicContaminationLocation
    {
        [Key]
        public int ScenarioArsenicContaminationLocationID { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string ScenarioArsenicContaminationLocationWellName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ScenarioArsenicContaminationLocationGeometry { get; set; }
    }
}
