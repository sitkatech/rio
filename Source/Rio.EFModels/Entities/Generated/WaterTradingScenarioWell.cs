using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class WaterTradingScenarioWell
    {
        [Key]
        public int WaterTradingScenarioWellID { get; set; }
        [Required]
        [StringLength(100)]
        public string WaterTradingScenarioWellCountyName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry WaterTradingScenarioWellGeometry { get; set; }
    }
}
