using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    [Table("ScenarioRechargeBasin")]
    [Index("ScenarioRechargeBasinName", "ScenarioRechargeBasinDisplayName", Name = "AK_ScenarioRechargeBasin_ScenarioRechargeBasinName_ScenarioRechargeBasinDisplayName", IsUnique = true)]
    public partial class ScenarioRechargeBasin
    {
        [Key]
        public int ScenarioRechargeBasinID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string ScenarioRechargeBasinName { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string ScenarioRechargeBasinDisplayName { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ScenarioRechargeBasinGeometry { get; set; }
    }
}
