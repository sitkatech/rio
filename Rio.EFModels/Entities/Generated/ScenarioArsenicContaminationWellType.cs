using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ScenarioArsenicContaminationWellType
    {
        public ScenarioArsenicContaminationWellType()
        {
            ScenarioArsenicContamination = new HashSet<ScenarioArsenicContamination>();
        }

        [Key]
        public int ScenarioArsenicContaminationWellTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationWellTypeName { get; set; }

        [InverseProperty("ScenarioArsenicContaminationWellType")]
        public virtual ICollection<ScenarioArsenicContamination> ScenarioArsenicContamination { get; set; }
    }
}
