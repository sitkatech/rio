using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class ScenarioRechargeBasin
    {
        [Key]
        public int ScenarioRechargeBasinID { get; set; }
        [Required]
        [StringLength(100)]
        public string ScenarioRechargeBasinName { get; set; }
        [Required]
        [StringLength(100)]
        public string ScenarioRechargeBasinDisplayName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ScenarioRechargeBasinGeometry { get; set; }
    }
}
