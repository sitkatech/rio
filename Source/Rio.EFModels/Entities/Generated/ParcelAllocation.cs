using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelAllocation")]
    [Index(nameof(ParcelID), nameof(WaterYear), nameof(ParcelAllocationTypeID), Name = "AK_ParcelAllocation_ParcelID_WaterYear", IsUnique = true)]
    public partial class ParcelAllocation
    {
        [Key]
        public int ParcelAllocationID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        public int ParcelAllocationTypeID { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal AcreFeetAllocated { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("ParcelAllocations")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(ParcelAllocationTypeID))]
        [InverseProperty("ParcelAllocations")]
        public virtual ParcelAllocationType ParcelAllocationType { get; set; }
    }
}
