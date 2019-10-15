using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class vGeoServerAllParcels
    {
        public int PrimaryKey { get; set; }
        public int ParcelID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerAddress { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerCity { get; set; }
        public int ParcelAreaInSquareFeet { get; set; }
        public double ParcelAreaInAcres { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ParcelGeometry { get; set; }
        public int? UserID { get; set; }
        [StringLength(201)]
        public string LandOwnerFullName { get; set; }
    }
}
