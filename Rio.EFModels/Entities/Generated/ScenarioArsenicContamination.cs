﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class ScenarioArsenicContamination
    {
        [Key]
        public int ScenarioArsenicContaminationID { get; set; }
        [Required]
        [StringLength(50)]
        public string ScenarioArsenicContaminationWellID { get; set; }
        public int ScenarioArsenicContaminationWellTypeID { get; set; }
        public int ScenarioArsenicContaminationSourceID { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ScenarioArsenicContaminationGeometry { get; set; }
        public double? ScenarioArsenicContaminationContaminationConcentration { get; set; }

        [ForeignKey(nameof(ScenarioArsenicContaminationSourceID))]
        [InverseProperty("ScenarioArsenicContamination")]
        public virtual ScenarioArsenicContaminationSource ScenarioArsenicContaminationSource { get; set; }
        [ForeignKey(nameof(ScenarioArsenicContaminationWellTypeID))]
        [InverseProperty("ScenarioArsenicContamination")]
        public virtual ScenarioArsenicContaminationWellType ScenarioArsenicContaminationWellType { get; set; }
    }
}
