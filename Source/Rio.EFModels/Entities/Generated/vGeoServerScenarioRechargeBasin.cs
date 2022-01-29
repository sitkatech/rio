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
    public partial class vGeoServerScenarioRechargeBasin
    {
        public int PrimaryKey { get; set; }
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
