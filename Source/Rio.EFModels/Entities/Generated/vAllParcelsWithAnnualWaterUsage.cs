using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vAllParcelsWithAnnualWaterUsage
    {
        public int ParcelID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public int? UserID { get; set; }
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        [Column(TypeName = "decimal(38, 4)")]
        public decimal? AllocationFor2016 { get; set; }
        [Column(TypeName = "decimal(38, 4)")]
        public decimal? AllocationFor2017 { get; set; }
        [Column(TypeName = "decimal(38, 4)")]
        public decimal? AllocationFor2018 { get; set; }
        [Column(TypeName = "decimal(38, 4)")]
        public decimal? WaterUsageFor2016 { get; set; }
        [Column(TypeName = "decimal(38, 4)")]
        public decimal? WaterUsageFor2017 { get; set; }
        [Column(TypeName = "decimal(38, 4)")]
        public decimal? WaterUsageFor2018 { get; set; }
    }
}
