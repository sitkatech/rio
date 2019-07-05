using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocation
    {
        public int ParcelAllocationID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal? AcreFeetAllocated { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelAllocation")]
        public virtual Parcel Parcel { get; set; }
    }
}
