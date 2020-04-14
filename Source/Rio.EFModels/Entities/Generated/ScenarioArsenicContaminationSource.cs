using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ScenarioArsenicContaminationSource
    {
        public ScenarioArsenicContaminationSource()
        {
            ScenarioArsenicContamination = new HashSet<ScenarioArsenicContamination>();
        }

        [Key]
        public int ScenarioArsenicContaminationSourceID { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationSourceName { get; set; }

        [InverseProperty("ScenarioArsenicContaminationSource")]
        public virtual ICollection<ScenarioArsenicContamination> ScenarioArsenicContamination { get; set; }
    }
}
