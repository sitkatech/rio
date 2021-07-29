using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ScenarioRechargeBasin")]
    [Index(nameof(ScenarioRechargeBasinName), nameof(ScenarioRechargeBasinDisplayName), Name = "AK_ScenarioRechargeBasin_ScenarioRechargeBasinName_ScenarioRechargeBasinDisplayName", IsUnique = true)]
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
