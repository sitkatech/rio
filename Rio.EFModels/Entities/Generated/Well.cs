using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    [Table("Well")]
    public partial class Well
    {
        [Key]
        public int WellID { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry WellGeometry { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string WellName { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string WellType { get; set; }
        public int WellTypeCode { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string WellTypeCodeName { get; set; }
    }
}
