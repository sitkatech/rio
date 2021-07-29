using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelUpdateStaging")]
    public partial class ParcelUpdateStaging
    {
        [Key]
        public int ParcelUpdateStagingID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry ParcelGeometry { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry ParcelGeometry4326 { get; set; }
        [Required]
        public string ParcelGeometryText { get; set; }
        [Required]
        public string ParcelGeometry4326Text { get; set; }
        public bool HasConflict { get; set; }
    }
}
